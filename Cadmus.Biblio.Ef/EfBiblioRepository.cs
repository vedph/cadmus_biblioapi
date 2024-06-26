﻿using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using LinqKit;
using LinqKit.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cadmus.Biblio.Ef;

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

    private BiblioDbContext GetContext() => new(_connectionString, DatabaseType);

    #region Works
    private static void PrepareWorkFilter(WorkFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.LastName))
            filter.LastName = StandardFilter.Apply(filter.LastName, true);

        if (!string.IsNullOrEmpty(filter.Title))
            filter.Title = StandardFilter.Apply(filter.Title, true);
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

        using var db = GetContext();
        IQueryable<EfWork> works = db.Works
            .AsNoTracking()
            .Include(w => w.Type)
            .Include(w => w.Container)
            .ThenInclude(w => w!.Type)
            .Include(w => w.AuthorWorks!)
            .ThenInclude(aw => aw.Author)
            .Include(w => w.KeywordWorks!)
            .ThenInclude(kw => kw.Keyword)
            .Include(w => w.Links);

        if (filter.IsMatchAnyEnabled)
        {
            // we need a predicate builder to chain clauses with OR
            // (note: this requires package LinqKit.Core)
            // http://www.albahari.com/nutshell/predicatebuilder.aspx

            var predicate = PredicateBuilder.New<EfWork>();

            if (!string.IsNullOrEmpty(filter.Key))
                predicate.Or(w => w.Key!.ToLower().Contains(filter.Key.ToLower()));

            if (!string.IsNullOrEmpty(filter.Type))
                predicate.Or(w => w.Type!.Equals(filter.Type));

            if (filter.AuthorId != Guid.Empty)
            {
                predicate.Or(w => w.AuthorWorks!.Any(
                    aw => aw.AuthorId == filter.AuthorId.ToString()));
            }

            if (!string.IsNullOrEmpty(filter.LastName))
            {
                predicate.Or(w =>
                    w.AuthorWorks!.Any(aw =>
                        aw.Author!.Lastx!.ToLower().Contains(filter.LastName.ToLower())));
            }

            if (!string.IsNullOrEmpty(filter.Language))
                predicate.Or(w => w.Language!.Equals(filter.Language));

            if (!string.IsNullOrEmpty(filter.Title))
                predicate.Or(w => w.Titlex!.ToLower().Contains(filter.Title.ToLower()));

            if (filter.ContainerId != Guid.Empty)
                predicate.Or(w => w.Container!.Id.Equals(filter.ContainerId));

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                predicate.Or(w => w.KeywordWorks!.Any(
                    kw => kw.Keyword!.Valuex!.Equals(filter.Keyword)));
            }

            if (filter.YearPubMin > 0)
                predicate.Or(w => w.YearPub >= filter.YearPubMin);

            if (filter.YearPubMax > 0)
                predicate.Or(w => w.YearPub <= filter.YearPubMax);

            if (filter.DatationMin != null)
                predicate.Or(w => w.DatationValue >= filter.DatationMin);

            if (filter.DatationMax != null)
                predicate.Or(w => w.DatationValue <= filter.DatationMax);

            works = works.AsExpandable().Where(predicate);
        }
        else
        {
            // key
            if (!string.IsNullOrEmpty(filter.Key))
                works = works.Where(w => w.Key!.ToLower().Contains(filter.Key.ToLower()));

            // type
            if (!string.IsNullOrEmpty(filter.Type))
                works = works.Where(w => w.Type!.Name == filter.Type);

            // author ID
            if (filter.AuthorId != Guid.Empty)
            {
                works = works.Where(w => w.AuthorWorks!.Any(
                    aw => aw.AuthorId == filter.AuthorId.ToString()));
            }

            // last
            if (!string.IsNullOrEmpty(filter.LastName))
            {
                works = works.Where(w => w.AuthorWorks!.Any(aw =>
                    aw.Author!.Lastx!.ToLower().Contains(filter.LastName.ToLower())));
            }

            // language
            if (!string.IsNullOrEmpty(filter.Language))
                works = works.Where(w => w.Language == filter.Language);

            // title
            if (!string.IsNullOrEmpty(filter.Title))
                works = works.Where(w => w.Titlex!.ToLower().Contains(filter.Title.ToLower()));

            // container ID
            if (filter.ContainerId != Guid.Empty)
            {
                works = works.Where(w =>
                    w.ContainerId!.Equals(filter.ContainerId));
            }

            // keyword
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                works = works.Where(w => w.KeywordWorks!.Any(
                    kw => kw.Keyword!.Valuex!.Equals(filter.Keyword)));
            }

            // yearpubmin
            if (filter.YearPubMin > 0)
                works = works.Where(w => w.YearPub >= filter.YearPubMin);

            // yearpubmax
            if (filter.YearPubMax > 0)
                works = works.Where(w => w.YearPub <= filter.YearPubMax);

            // datationmin
            if (filter.DatationMin != null)
                works = works.Where(w => w.DatationValue >= filter.DatationMin);

            // datationmax
            if (filter.DatationMax != null)
                works = works.Where(w => w.DatationValue <= filter.DatationMax);
        }

        int tot = works.Count();

        // sort and page
        works = works
            .OrderBy(w => w.AuthorWorks!.Select(aw => aw.Author)
                .First()!.Lastx)
            .ThenBy(w => w.AuthorWorks!.Select(aw => aw.Author)
                .First()!.First)
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

    /// <summary>
    /// Gets the work by its ID.
    /// </summary>
    /// <param name="id">The work's identifier.</param>
    /// <returns>
    /// Work, or null if not found
    /// </returns>
    public Work? GetWork(Guid id)
    {
        using var db = GetContext();
        EfWork? work = db.Works
            .AsNoTracking()
            .Include(w => w.Type)
            .Include(w => w.Container)
            .ThenInclude(w => w!.Type)
            .Include(w => w.AuthorWorks!)
            .ThenInclude(aw => aw.Author)
            .Include(w => w.KeywordWorks!)
            .ThenInclude(kw => kw.Keyword)
            .Include(w => w.Links)
            .FirstOrDefault(w => w.Id == id.ToString());
        return EfHelper.GetWork(work);
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
    /// If new, <paramref name="work"/> ID gets updated; the key is
    /// always updated.
    /// </summary>
    /// <param name="work">The work.</param>
    /// <exception cref="ArgumentNullException">work</exception>
    public void AddWork(Work work)
    {
        if (work == null) throw new ArgumentNullException(nameof(work));

        using var db = GetContext();
        EfWork? ef = EfHelper.GetEfWork(work, db);
        db.SaveChanges();
        if (ef != null)
        {
            work.Id = Guid.Parse(ef.Id);
            work.Key = ef.Key;
        }
    }

    /// <summary>
    /// Deletes the work with the specified ID.
    /// </summary>
    /// <param name="id">The work's identifier.</param>
    public void DeleteWork(Guid id)
    {
        using var db = GetContext();
        EfWork? work = db.Works.Find(id.ToString());
        if (work != null)
        {
            db.Works.Remove(work);
            db.SaveChanges();
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

        using var db = GetContext();
        IQueryable<EfContainer> containers = db.Containers
            .AsNoTracking()
            .Include(c => c.Type)
            .Include(c => c.AuthorContainers!)
            .ThenInclude(ac => ac.Author)
            .Include(c => c.KeywordContainers!)
            .ThenInclude(kc => kc.Keyword)
            .Include(c => c.Links);

        if (filter.IsMatchAnyEnabled)
        {
            // we need a predicate builder to chain clauses with OR
            // (note: this requires package LinqKit.Core)
            // http://www.albahari.com/nutshell/predicatebuilder.aspx

            var predicate = PredicateBuilder.New<EfContainer>();

            if (!string.IsNullOrEmpty(filter.Key))
                predicate.Or(c => c.Key!.ToLower().Contains(filter.Key.ToLower()));

            if (!string.IsNullOrEmpty(filter.Type))
                predicate.Or(c => c.Type!.Equals(filter.Type));

            if (filter.AuthorId != Guid.Empty)
            {
                predicate.Or(c => c.AuthorContainers!.Any(
                    ac => ac.AuthorId == filter.AuthorId.ToString()));
            }

            if (!string.IsNullOrEmpty(filter.LastName))
            {
                predicate.Or(c =>
                    c.AuthorContainers!.Any(ac => ac.Author!.Lastx!.ToLower()
                        .Contains(filter.LastName.ToLower())));
            }

            if (!string.IsNullOrEmpty(filter.Language))
                predicate.Or(c => c.Language!.Equals(filter.Language));

            if (!string.IsNullOrEmpty(filter.Title))
                predicate.Or(c => c.Titlex!.ToLower().Contains(
                    filter.Title.ToLower()));

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                predicate.Or(c => c.KeywordContainers!.Any(
                    kc => kc.Keyword!.Valuex!.Equals(filter.Keyword)));
            }

            if (filter.YearPubMin > 0)
                predicate.Or(c => c.YearPub >= filter.YearPubMin);

            if (filter.YearPubMax > 0)
                predicate.Or(c => c.YearPub <= filter.YearPubMax);

            if (filter.DatationMin != null)
                predicate.Or(w => w.DatationValue >= filter.DatationMin);

            if (filter.DatationMax != null)
                predicate.Or(w => w.DatationValue <= filter.DatationMax);

            containers = containers.AsExpandable().Where(predicate);
        }
        else
        {
            // key
            if (!string.IsNullOrEmpty(filter.Key))
                containers = containers.Where(w => w.Key!.ToLower().Contains(filter.Key.ToLower()));

            // type
            if (!string.IsNullOrEmpty(filter.Type))
                containers = containers.Where(w => w.Type!.Name == filter.Type);

            // author ID
            if (filter.AuthorId != Guid.Empty)
            {
                containers = containers.Where(c => c.AuthorContainers!.Any(
                    ac => ac.AuthorId == filter.AuthorId.ToString()));
            }

            // last
            if (!string.IsNullOrEmpty(filter.LastName))
            {
                containers = containers.Where(w => w.AuthorContainers!.Any(aw =>
                   aw.Author != null && aw.Author.Lastx!.Contains(filter.LastName)));
            }

            // language
            if (!string.IsNullOrEmpty(filter.Language))
                containers = containers.Where(w => w.Language == filter.Language);

            // title
            if (!string.IsNullOrEmpty(filter.Title))
                containers = containers.Where(w => w.Titlex!.ToLower().Contains(filter.Title.ToLower()));

            // keyword
            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                containers = containers.Where(w => w.KeywordContainers!.Any(
                    kw => kw.Keyword!.Valuex!.Equals(filter.Keyword)));
            }

            // yearpubmin
            if (filter.YearPubMin > 0)
                containers = containers.Where(w => w.YearPub >= filter.YearPubMin);

            // yearpubmax
            if (filter.YearPubMax > 0)
                containers = containers.Where(w => w.YearPub <= filter.YearPubMax);

            // datationmin
            if (filter.DatationMin != null)
                containers = containers.Where(w => w.DatationValue >= filter.DatationMin);

            // datationmax
            if (filter.DatationMax != null)
                containers = containers.Where(w => w.DatationValue <= filter.DatationMax);
        }

        int tot = containers.Count();

        // sort and page
        containers = containers
            .OrderBy(c => c.AuthorContainers!.Select(ac => ac.Author).First()!.Lastx)
            .ThenBy(c => c.AuthorContainers!.Select(ac => ac.Author).First()!.First)
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

    /// <summary>
    /// Gets the container by its ID.
    /// </summary>
    /// <param name="id">The container's identifier.</param>
    /// <returns>
    /// Container, or null if not found
    /// </returns>
    public Container? GetContainer(Guid id)
    {
        using var db = GetContext();
        EfContainer? container = db.Containers
            .AsNoTracking()
            .Include(c => c.Type)
            .Include(c => c.AuthorContainers!)
            .ThenInclude(ac => ac.Author)
            .Include(c => c.KeywordContainers!)
            .ThenInclude(kc => kc.Keyword)
            .Include(c => c.Links)
            .FirstOrDefault(c => c.Id == id.ToString());
        return EfHelper.GetContainer(container);
    }

    /// <summary>
    /// Adds or updates the specified container.
    /// Container type, authors, and keywords are stored too.
    /// As for authors, you can specify only the author's ID to assign
    /// the work to an existing author; otherwise, the author will be
    /// either added or updated as required. Keywords are added or
    /// updated as required. Type is added if not found, even this should
    /// not happen, as types are a predefined set.
    /// If new, <paramref name="container"/> ID gets updated; the key is
    /// always updated.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <exception cref="ArgumentNullException">container</exception>
    public void AddContainer(Container container)
    {
        if (container == null) throw new ArgumentNullException(nameof(container));

        using var db = GetContext();
        EfContainer? ef = EfHelper.GetEfContainer(container, db);
        db.SaveChanges();
        if (ef != null)
        {
            container.Id = Guid.Parse(ef.Id);
            container.Key = ef.Key;
        }
    }

    /// <summary>
    /// Deletes the container.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void DeleteContainer(Guid id)
    {
        using var db = GetContext();
        EfContainer? container = db.Containers.Find(id.ToString());
        if (container != null)
        {
            db.Containers.Remove(container);
            db.SaveChanges();
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
    public WorkType? GetWorkType(string id)
    {
        if (id == null)
            throw new ArgumentNullException(nameof(id));

        using var db = GetContext();
        EfWorkType? ef = db.WorkTypes.Find(id.ToString());
        return EfHelper.GetWorkType(ef);
    }

    /// <summary>
    /// Gets the page of types matching the specified filter,
    /// or all of them when page size is 0.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The types page.</returns>
    /// <exception cref="ArgumentNullException">filter</exception>
    public DataPage<WorkType> GetWorkTypes(WorkTypeFilter filter)
    {
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        using var db = GetContext();
        IQueryable<EfWorkType> types = db.WorkTypes.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Name))
            types = types.Where(t => t.Name!.ToLower().Contains(filter.Name.ToLower()));

        int tot = types.Count();

        // sort and page
        types = types.OrderBy(t => t.Name).ThenBy(t => t.Id);
        List<EfWorkType> pgTypes;
        if (filter.PageSize > 0)
        {
            pgTypes = types.Skip(filter.GetSkipCount())
                .Take(filter.PageSize)
                .ToList();
        }
        else
        {
            pgTypes = types.Skip(filter.GetSkipCount()).ToList();
        }

        return new DataPage<WorkType>(
            filter.PageNumber,
            filter.PageSize,
            tot,
            (from t in pgTypes select EfHelper.GetWorkType(t)).ToList());
    }

    /// <summary>
    /// Adds or updates the type with the specified ID and name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <exception cref="ArgumentNullException">type</exception>
    public void AddWorkType(WorkType type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        using var db = GetContext();
        EfHelper.EnsureEfWorkType(type, db);
        db.SaveChanges();
    }

    /// <summary>
    /// Deletes the type with the specified ID.
    /// </summary>
    /// <param name="id">The ID.</param>
    public void DeleteWorkType(string id)
    {
        using var db = GetContext();
        EfWorkType? ef = db.WorkTypes.Find(id.ToString());
        if (ef != null)
        {
            db.WorkTypes.Remove(ef);
            db.SaveChanges();
        }
    }
    #endregion

    #region Authors
    private static void PrepareAuthorFilter(AuthorFilter filter)
    {
        if (!string.IsNullOrEmpty(filter.Last))
            filter.Last = StandardFilter.Apply(filter.Last!, true);
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
        if (filter == null) throw new ArgumentNullException(nameof(filter));

        PrepareAuthorFilter(filter);

        using var db = GetContext();
        var authors = db.Authors.AsQueryable();
        if (!string.IsNullOrEmpty(filter.Last))
        {
            authors = authors.Where(
                a => a.Lastx!.ToLower().Contains(filter.Last.ToLower()));
        }

        int total = authors.Count();
        if (total == 0)
        {
            return new DataPage<Author>(filter.PageNumber, filter.PageSize,
                0, Array.Empty<Author>());
        }

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
            total,
            (from a in pgAuthors select EfHelper.GetAuthor(a)).ToList());
    }

    /// <summary>
    /// Gets the author with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>
    /// The author, or null if not found.
    /// </returns>
    public Author? GetAuthor(Guid id)
    {
        using var db = GetContext();
        EfAuthor? ef = db.Authors.Find(id.ToString());
        return EfHelper.GetAuthor(ef);
    }

    /// <summary>
    /// Adds or updates the specified author.
    /// If the ID is not specified, a new author will be created and
    /// the <paramref name="author"/>'s ID will be updated.
    /// </summary>
    /// <param name="author">The author.</param>
    /// <exception cref="ArgumentNullException">author</exception>
    public void AddAuthor(Author author)
    {
        if (author == null) throw new ArgumentNullException(nameof(author));

        using var db = GetContext();
        EfAuthor? ef = EfHelper.GetOrAddEfAuthor(author, db);
        db.SaveChanges();
        if (ef != null) author.Id = Guid.Parse(ef.Id);
    }

    /// <summary>
    /// Deletes the author with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void DeleteAuthor(Guid id)
    {
        using var db = GetContext();
        EfAuthor? ef = db.Authors.Find(id.ToString());
        if (ef != null)
        {
            db.Authors.Remove(ef);
            db.SaveChanges();
        }
    }

    /// <summary>
    /// Prunes the authors by removing all the authors not assigned to
    /// any work or container.
    /// </summary>
    public void PruneAuthors()
    {
        using var db = GetContext();
        var authors = from a in db.Authors
                      where db.AuthorWorks.All(aw => aw.AuthorId != a.Id)
                        && db.AuthorContainers.All(ac => ac.AuthorId != a.Id)
                      select a;
        db.Authors.RemoveRange(authors);
        db.SaveChanges();
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
    public Keyword? GetKeyword(int id)
    {
        using var db = GetContext();
        EfKeyword? ef = db.Keywords.Find(id);
        return ef != null ? EfHelper.GetKeyword(ef) : null;
    }

    /// <summary>
    /// Deletes the keyword with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void DeleteKeyword(int id)
    {
        using var db = GetContext();
        EfKeyword? ef = db.Keywords.Find(id);
        if (ef != null)
        {
            db.Keywords.Remove(ef);
            db.SaveChanges();
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

        using var db = GetContext();
        var keywords = db.Keywords.AsQueryable();

        if (!string.IsNullOrEmpty(filter.Language))
            keywords = keywords.Where(k => k.Language == filter.Language);

        if (!string.IsNullOrEmpty(filter.Value))
        {
            // filter value for keyword can be language:value
            int i = filter.Value.IndexOf(':');
            if (i == 3)
            {
                string l = filter.Value[..3];
                keywords = keywords.Where(k => k.Language == l);
                if (filter.Value.Length > 4)
                {
                    string v = filter.Value[4..];
                    keywords = keywords.Where(k => k.Valuex!.Contains(v));
                }
            }
            else
            {
                keywords = keywords.Where(k =>
                    k.Valuex!.ToLower().Contains(filter.Value.ToLower()));
            }
        }

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

        using var db = GetContext();
        EfKeyword? ef = EfHelper.GetEfKeyword(keyword, db);
        db.SaveChanges();
        return ef!.Id;
    }

    /// <summary>
    /// Prunes the keywords by removing all the keywords not assigned to
    /// any work.
    /// </summary>
    public void PruneKeywords()
    {
        using var db = GetContext();
        var keywords = from k in db.Keywords
                       where db.KeywordWorks.All(kw => kw.KeywordId != k.Id)
                         && db.KeywordContainers.All(kc => kc.KeywordId != k.Id)
                       select k;
        db.Keywords.RemoveRange(keywords);
        db.SaveChanges();
    }
    #endregion
}
