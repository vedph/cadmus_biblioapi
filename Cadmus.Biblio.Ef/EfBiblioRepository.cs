using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Entity-framework core based bibliographic repository.
    /// </summary>
    /// <seealso cref="IBiblioRepository" />
    public sealed class EfBiblioRepository : IBiblioRepository
    {
        private readonly string _connectionString;

        /// <summary>
        /// Gets the type of the database.
        /// </summary>
        public string DatabaseType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfBiblioRepository" />
        /// class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseType">The database type.</param>
        /// <exception cref="ArgumentNullException">options</exception>
        public EfBiblioRepository(string connectionString, string databaseType)
        {
            _connectionString = connectionString ??
                throw new ArgumentNullException(nameof(connectionString));
            DatabaseType = databaseType;
        }

        private BiblioDbContext GetContext() =>
            new BiblioDbContext(_connectionString, DatabaseType);

        #region Works
        private void PrepareWorkFilter(WorkFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LastName))
                filter.LastName = StandardFilter.Apply(filter.LastName, true);

            if (!string.IsNullOrEmpty(filter.Title))
                filter.Title = StandardFilter.Apply(filter.Title, true);

            if (!string.IsNullOrEmpty(filter.ContainerTitle))
                filter.ContainerTitle = StandardFilter.Apply(filter.ContainerTitle, true);
        }

        /// <summary>
        /// Gets the specified page of filtered works.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The works page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<WorkInfo> GetWorks(WorkFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            PrepareWorkFilter(filter);

            using (var db = GetContext())
            {
                IQueryable<EfWork> works = db.Works
                    .AsNoTracking()
                    .Include(w => w.Type)
                    .Include(w => w.Container)
                    .Include(w => w.AuthorWorks)
                    .ThenInclude(aw => aw.Author)
                    .Include(w => w.KeywordWorks)
                    .ThenInclude(kw => kw.Keyword);

                if (filter.IsMatchAnyEnabled)
                {
                    works = works.Where(w =>
                        // key
                        string.IsNullOrEmpty(filter.Key) || w.Key == filter.Key
                        // type
                        || string.IsNullOrEmpty(filter.Type)
                            || w.Type.Name == filter.Type
                        // last
                        || string.IsNullOrEmpty(filter.LastName)
                            || w.AuthorWorks.Any(
                                aw => aw.Author.Lastx.Contains(filter.LastName))
                        // language
                        || string.IsNullOrEmpty(filter.Language)
                            || w.Language == filter.Language
                        // title
                        || string.IsNullOrEmpty(filter.Title)
                            || w.Titlex.Contains(filter.Title)
                        // container
                        || string.IsNullOrEmpty(filter.ContainerTitle)
                            || w.Container.Titlex.Contains(filter.ContainerTitle)
                        // keyword
                        || string.IsNullOrEmpty(filter.Keyword)
                            || w.KeywordWorks.Any(
                                kw => kw.Keyword.Valuex.Equals(filter.Keyword))
                        // yearpubmin
                        || filter.YearPubMin == 0 || w.YearPub >= filter.YearPubMin
                        // yearpubmax
                        || filter.YearPubMax == 0 || w.YearPub <= filter.YearPubMax);
                }
                else
                {
                    // key
                    if (!string.IsNullOrEmpty(filter.Key))
                        works = works.Where(w => w.Key == filter.Key);

                    // type
                    if (!string.IsNullOrEmpty(filter.Type))
                        works = works.Where(w => w.Type.Name == filter.Type);

                    // last
                    if (!string.IsNullOrEmpty(filter.LastName))
                    {
                        works = works.Where(w => w.AuthorWorks.Any(
                            aw => aw.Author.Lastx.Contains(filter.LastName)));
                    }

                    // language
                    if (!string.IsNullOrEmpty(filter.Language))
                        works = works.Where(w => w.Language == filter.Language);

                    // title
                    if (!string.IsNullOrEmpty(filter.Title))
                        works = works.Where(w => w.Titlex.Contains(filter.Title));

                    // container
                    if (!string.IsNullOrEmpty(filter.ContainerTitle))
                    {
                        works = works.Where(w => w.Container.Titlex.Contains(
                            filter.ContainerTitle));
                    }

                    // keyword
                    if (!string.IsNullOrEmpty(filter.Keyword))
                    {
                        works = works.Where(w => w.KeywordWorks.Any(
                            kw => kw.Keyword.Valuex.Equals(filter.Keyword)));
                    }

                    // yearpubmin
                    if (filter.YearPubMin > 0)
                        works = works.Where(w => w.YearPub >= filter.YearPubMin);

                    // yearpubmax
                    if (filter.YearPubMax > 0)
                        works = works.Where(w => w.YearPub <= filter.YearPubMax);
                }

                int tot = works.Count();

                // sort and page
                works = works.OrderBy(w => w.AuthorWorks[0].Author.Lastx)
                    .ThenBy(w => w.AuthorWorks[0].Author.First)
                    .ThenBy(w => w.Titlex)
                    .ThenBy(w => w.Key)
                    .ThenBy(w => w.Id);
                var pgWorks = works.Skip(filter.GetSkipCount())
                    .Take(filter.PageSize)
                    .ToList();

                return new DataPage<WorkInfo>(
                    filter.PageNumber,
                    filter.PageSize,
                    tot,
                    (from w in pgWorks select EfHelper.GetWorkInfo(w, db)).ToList());
            }
        }

        /// <summary>
        /// Gets the work by its ID.
        /// </summary>
        /// <param name="id">The work's identifier.</param>
        /// <returns>
        /// Work, or null if not found
        /// </returns>
        public Work GetWork(Guid id)
        {
            using (var db = GetContext())
            {
                EfWork work = db.Works
                    .AsNoTracking()
                    .Include(w => w.Type)
                    .Include(w => w.Container)
                    .Include(w => w.AuthorWorks)
                    .ThenInclude(aw => aw.Author)
                    .Include(w => w.KeywordWorks)
                    .ThenInclude(kw => kw.Keyword)
                    .FirstOrDefault(w => w.Id == id);
                return EfHelper.GetWork(work);
            }
        }

        /// <summary>
        /// Adds or updates the specified work.
        /// Work type, container, authors, and keywords are stored too.
        /// As for authors, you can specify only the author's ID to assign
        /// the work to an existing author; otherwise, the author will be
        /// either added or updated as required. Keywords are added or
        /// updated as required. Type is added if not found, even this should
        /// not happen, as types are a predefined set. As for the container,
        /// you can specify only its ID to assign the work to it; otherwise,
        /// the container will be added or updated as required.
        /// </summary>
        /// <param name="work">The work.</param>
        /// <exception cref="ArgumentNullException">work</exception>
        public void AddWork(Work work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));

            using (var db = GetContext())
            {
                EfWork ef = EfHelper.GetEfWork(work, db);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the work with the specified ID.
        /// </summary>
        /// <param name="id">The work's identifier.</param>
        public void DeleteWork(Guid id)
        {
            using (var db = GetContext())
            {
                EfWork work = db.Works.Find(id);
                if (work != null)
                {
                    db.Works.Remove(work);
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region Containers
        /// <summary>
        /// Gets the specified page of filtered containers.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The containers page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<WorkInfo> GetContainers(WorkFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            PrepareWorkFilter(filter);

            using (var db = GetContext())
            {
                IQueryable<EfContainer> containers = db.Containers
                    .AsNoTracking()
                    .Include(w => w.Type)
                    .Include(w => w.AuthorContainers)
                    .ThenInclude(aw => aw.Author)
                    .Include(w => w.KeywordContainers)
                    .ThenInclude(kw => kw.Keyword);

                if (filter.IsMatchAnyEnabled)
                {
                    containers = containers.Where(c =>
                        // key
                        string.IsNullOrEmpty(filter.Key) || c.Key == filter.Key
                        // type
                        || string.IsNullOrEmpty(filter.Type)
                            || c.Type.Name == filter.Type
                        // last
                        || string.IsNullOrEmpty(filter.LastName)
                            || c.AuthorContainers.Any(
                                aw => aw.Author.Lastx.Contains(filter.LastName))
                        // language
                        || string.IsNullOrEmpty(filter.Language)
                            || c.Language == filter.Language
                        // title
                        || string.IsNullOrEmpty(filter.Title)
                            || c.Titlex.Contains(filter.Title)
                        // keyword
                        || string.IsNullOrEmpty(filter.Keyword)
                            || c.KeywordContainers.Any(
                                kw => kw.Keyword.Valuex.Equals(filter.Keyword))
                        // yearpubmin
                        || filter.YearPubMin == 0 || c.YearPub >= filter.YearPubMin
                        // yearpubmax
                        || filter.YearPubMax == 0 || c.YearPub <= filter.YearPubMax);
                }
                else
                {
                    // key
                    if (!string.IsNullOrEmpty(filter.Key))
                        containers = containers.Where(w => w.Key == filter.Key);

                    // type
                    if (!string.IsNullOrEmpty(filter.Type))
                        containers = containers.Where(w => w.Type.Name == filter.Type);

                    // last
                    if (!string.IsNullOrEmpty(filter.LastName))
                    {
                        containers = containers.Where(w => w.AuthorContainers.Any(
                            aw => aw.Author.Lastx.Contains(filter.LastName)));
                    }

                    // language
                    if (!string.IsNullOrEmpty(filter.Language))
                        containers = containers.Where(w => w.Language == filter.Language);

                    // title
                    if (!string.IsNullOrEmpty(filter.Title))
                        containers = containers.Where(w => w.Titlex.Contains(filter.Title));

                    // keyword
                    if (!string.IsNullOrEmpty(filter.Keyword))
                    {
                        containers = containers.Where(w => w.KeywordContainers.Any(
                            kw => kw.Keyword.Valuex.Equals(filter.Keyword)));
                    }

                    // yearpubmin
                    if (filter.YearPubMin > 0)
                        containers = containers.Where(w => w.YearPub >= filter.YearPubMin);

                    // yearpubmax
                    if (filter.YearPubMax > 0)
                        containers = containers.Where(w => w.YearPub <= filter.YearPubMax);
                }

                int tot = containers.Count();

                // sort and page
                containers = containers
                    .OrderBy(c => c.AuthorContainers.Select(ac => ac.Author).First().Lastx)
                    .ThenBy(c => c.AuthorContainers.Select(ac => ac.Author).First().First)
                    .ThenBy(c => c.Titlex)
                    .ThenBy(c => c.Key)
                    .ThenBy(c => c.Id);
                var pgContainers = containers.Skip(filter.GetSkipCount())
                    .Take(filter.PageSize)
                    .ToList();

                return new DataPage<WorkInfo>(
                    filter.PageNumber,
                    filter.PageSize,
                    tot,
                    (from w in pgContainers select EfHelper.GetWorkInfo(w, db)).ToList());
            }
        }

        /// <summary>
        /// Gets the container by its ID.
        /// </summary>
        /// <param name="id">The container's identifier.</param>
        /// <returns>
        /// Container, or null if not found
        /// </returns>
        public Container GetContainer(Guid id)
        {
            using (var db = GetContext())
            {
                EfContainer container = db.Containers
                    .AsNoTracking()
                    .Include(w => w.Type)
                    .Include(w => w.AuthorContainers)
                    .ThenInclude(aw => aw.Author)
                    .Include(w => w.KeywordContainers)
                    .ThenInclude(kw => kw.Keyword)
                    .FirstOrDefault(w => w.Id == id);
                return EfHelper.GetContainer(container);
            }
        }

        /// <summary>
        /// Adds or updates the specified container.
        /// Container type, authors, and keywords are stored too.
        /// As for authors, you can specify only the author's ID to assign
        /// the work to an existing author; otherwise, the author will be
        /// either added or updated as required. Keywords are added or
        /// updated as required. Type is added if not found, even this should
        /// not happen, as types are a predefined set.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <exception cref="ArgumentNullException">container</exception>
        public void AddContainer(Container container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            using (var db = GetContext())
            {
                EfContainer ef = EfHelper.GetEfContainer(container, db);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the container.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteContainer(Guid id)
        {
            using (var db = GetContext())
            {
                EfContainer container = db.Containers.Find(id);
                if (container != null)
                {
                    db.Containers.Remove(container);
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region Types
        /// <summary>
        /// Gets the type with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The type, or null if not found.</returns>
        /// <exception cref="ArgumentNullException">id</exception>
        public WorkType GetType(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            using (var db = GetContext())
            {
                EfWorkType ef = db.WorkTypes.Find(id);
                return EfHelper.GetWorkType(ef);
            }
        }

        /// <summary>
        /// Gets the page of types matching the specified filter,
        /// or all of them when page size is 0.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The types page.</returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<WorkType> GetTypes(WorkTypeFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            using (var db = GetContext())
            {
                IQueryable<EfWorkType> types = db.WorkTypes.AsQueryable();

                if (!string.IsNullOrEmpty(filter.Name))
                    types = types.Where(t => t.Name.Contains(filter.Name));

                int tot = types.Count();

                // sort and page
                types = types.OrderBy(t => t.Name).ThenBy(t => t.Id);
                var pgTypes = types.Skip(filter.GetSkipCount())
                    .Take(filter.PageSize)
                    .ToList();

                return new DataPage<WorkType>(
                    filter.PageNumber,
                    filter.PageSize,
                    tot,
                    (from t in pgTypes select EfHelper.GetWorkType(t)).ToList());
            }
        }

        /// <summary>
        /// Adds or updates the type with the specified ID and name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">type</exception>
        public void AddType(WorkType type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            using (var db = GetContext())
            {
                EfWorkType ef = EfHelper.GetEfWorkType(type, db);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the type with the specified ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        public void DeleteType(string id)
        {
            using (var db = GetContext())
            {
                EfWorkType ef = db.WorkTypes.Find(id);
                if (ef != null)
                {
                    db.WorkTypes.Remove(ef);
                    db.SaveChanges();
                }
            }
        }
        #endregion

        #region Authors
        private static void PrepareAuthorFilter(AuthorFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Last))
                filter.Last = StandardFilter.Apply(filter.Last, true);
        }

        /// <summary>
        /// Gets the specified page of authors matching <paramref name="filter" />.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// Page of authors.
        /// </returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<Author> GetAuthors(AuthorFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            PrepareAuthorFilter(filter);

            using (var db = GetContext())
            {
                var authors = db.Authors.AsQueryable();
                if (!string.IsNullOrEmpty(filter.Last))
                    authors = authors.Where(a => a.Lastx.Contains(filter.Last));

                int tot = authors.Count();

                // sort and page
                authors = authors.OrderBy(a => a.Last)
                    .ThenBy(a => a.First)
                    .ThenBy(a => a.Suffix)
                    .ThenBy(a => a.Id);
                var pgAuthors = authors.Skip(filter.GetSkipCount())
                    .Take(filter.PageSize)
                    .ToList();

                return new DataPage<Author>(
                    filter.PageNumber,
                    filter.PageSize,
                    tot,
                    (from a in pgAuthors select EfHelper.GetAuthor(a)).ToList());
            }
        }

        /// <summary>
        /// Gets the author with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// The author, or null if not found.
        /// </returns>
        public Author GetAuthor(Guid id)
        {
            using (var db = GetContext())
            {
                EfAuthor ef = db.Authors.Find(id);
                return EfHelper.GetAuthor(ef);
            }
        }

        /// <summary>
        /// Adds or updates the specified author.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <exception cref="ArgumentNullException">author</exception>
        public void AddAuthor(Author author)
        {
            if (author == null)
                throw new ArgumentNullException(nameof(author));

            using (var db = GetContext())
            {
                EfAuthor ef = EfHelper.GetEfAuthor(author, db);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Deletes the author with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void DeleteAuthor(Guid id)
        {
            using (var db = GetContext())
            {
                EfAuthor ef = db.Authors.Find(id);
                if (ef != null)
                {
                    db.Authors.Remove(ef);
                    db.SaveChanges();
                }
            }
        }

        /// <summary>
        /// Prunes the authors by removing all the authors not assigned to
        /// any work or container.
        /// </summary>
        public void PruneAuthors()
        {
            using (var db = GetContext())
            {
                var authors = from a in db.Authors
                              where db.AuthorWorks.All(aw => aw.AuthorId != a.Id)
                                && db.AuthorContainers.All(ac => ac.AuthorId != a.Id)
                              select a;
                db.Authors.RemoveRange(authors);
                db.SaveChanges();
            }
        }
        #endregion

        #region Keywords
        private static void PrepareKeywordFilter(KeywordFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Value))
                filter.Value = StandardFilter.Apply(filter.Value, true);
        }

        /// <summary>
        /// Gets the keyword with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The keyword or null if not found.</returns>
        public Keyword GetKeyword(int id)
        {
            using (var db = GetContext())
            {
                EfKeyword ef = db.Keywords.Find(id);
                return ef != null ? EfHelper.GetKeyword(ef) : null;
            }
        }

        /// <summary>
        /// Gets the specified page of keywords.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The page.</returns>
        public DataPage<Keyword> GetKeywords(KeywordFilter filter)
        {
            PrepareKeywordFilter(filter);

            using (var db = GetContext())
            {
                var keywords = db.Keywords.AsQueryable();

                if (!string.IsNullOrEmpty(filter.Language))
                    keywords = keywords.Where(k => k.Language == filter.Language);

                if (!string.IsNullOrEmpty(filter.Value))
                    keywords = keywords.Where(k => k.Valuex.Contains(filter.Value));

                int tot = keywords.Count();

                // sort and page
                keywords = keywords.OrderBy(k => k.Language)
                    .ThenBy(k => k.Value)
                    .ThenBy(k => k.Id);
                keywords = keywords.Skip(filter.GetSkipCount()).Take(filter.PageSize);

                return new DataPage<Keyword>(
                    filter.PageNumber,
                    filter.PageSize,
                    tot,
                    (from k in keywords select EfHelper.GetKeyword(k)).ToList());
            }
        }

        /// <summary>
        /// Adds or updates the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>The keyword's ID.</returns>
        /// <exception cref="ArgumentNullException">keyword</exception>
        public int AddKeyword(Keyword keyword)
        {
            if (keyword == null)
                throw new ArgumentNullException(nameof(keyword));

            using (var db = GetContext())
            {
                EfKeyword ef = EfHelper.GetEfKeyword(keyword, db);
                db.SaveChanges();
                return ef.Id;
            }
        }

        /// <summary>
        /// Prunes the keywords by removing all the keywords not assigned to
        /// any work.
        /// </summary>
        public void PruneKeywords()
        {
            using (var db = GetContext())
            {
                var keywords = from k in db.Keywords
                              where db.KeywordWorks.All(kw => kw.KeywordId != k.Id)
                                && db.KeywordContainers.All(kc => kc.KeywordId != k.Id)
                              select k;
                db.Keywords.RemoveRange(keywords);
                db.SaveChanges();
            }
        }
        #endregion
    }
}
