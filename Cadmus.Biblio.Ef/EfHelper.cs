using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Helper class for repositories.
    /// </summary>
    public static class EfHelper
    {
        /// <summary>
        /// The manual-key prefix. When a work or container key starts with
        /// this prefix, it is not automatically updated by the system.
        /// </summary>
        public const string MAN_KEY_PREFIX = "!";

        /// <summary>
        /// Gets the MySql schema for the bibliographic database.
        /// </summary>
        /// <returns>SQL code.</returns>
        public static string GetSchema()
        {
            using (StreamReader reader = new StreamReader(
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(
                    "Cadmus.Biblio.Ef.Assets.cadmus-biblio.sql")))
            {
                return reader.ReadToEnd();
            }
        }

        #region Entity to POCO        
        /// <summary>
        /// Gets the work's type corresponding to the specified work type entity.
        /// </summary>
        /// <param name="ef">The entity or null.</param>
        /// <returns>The work type or null.</returns>
        public static WorkType GetWorkType(EfWorkType ef)
        {
            if (ef == null) return null;
            return new WorkType
            {
                Id = ef.Id,
                Name = ef.Name
            };
        }

        /// <summary>
        /// Gets the keyword corresponding to the specified keyword entity.
        /// </summary>
        /// <param name="ef">The keyword or null.</param>
        /// <returns>The keyword or null.</returns>
        public static Keyword GetKeyword(EfKeyword ef)
        {
            if (ef == null) return null;
            return new Keyword
            {
                Language = ef.Language,
                Value = ef.Value
            };
        }

        /// <summary>
        /// Gets the author corresponding to the specified EF author.
        /// </summary>
        /// <param name="ef">The author or null.</param>
        /// <returns>The author or null.</returns>
        public static Author GetAuthor(EfAuthor ef)
        {
            if (ef == null) return null;

            return new Author
            {
                Id = ef.Id,
                First = ef.First,
                Last = ef.Last,
                Suffix = ef.Suffix
            };
        }

        /// <summary>
        /// Gets the work information corresponding to the specified container
        /// entity.
        /// </summary>
        /// <param name="ef">The entity or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The object or null.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public static WorkInfo GetWorkInfo(EfContainer ef, BiblioDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (ef == null) return null;
            WorkInfo info = new WorkInfo
            {
                Id = ef.Id,
                Key = ef.Key,
                Type = ef.Type?.Name,
                Title = ef.Title,
                Language = ef.Language,
                Edition = ef.Edition,
                YearPub = ef.YearPub,
                PlacePub = ef.PlacePub,
                Number = ef.Number
            };

            // authors
            if (ef.AuthorContainers?.Count > 0)
            {
                info.Authors = (from aw in ef.AuthorContainers
                                select new WorkAuthor
                                {
                                    First = aw.Author?.First,
                                    Last = aw.Author?.Last,
                                    Role = aw.Role
                                }).ToList();
            }

            // keywords
            if (ef.KeywordContainers?.Count > 0)
            {
                info.Keywords = (from kw in ef.KeywordContainers
                                 select new Keyword
                                 {
                                     Language = kw.Keyword?.Language,
                                     Value = kw.Keyword?.Value
                                 }).ToList();
            }

            return info;
        }

        /// <summary>
        /// Gets the work information corresponding to the specified work
        /// entity.
        /// </summary>
        /// <param name="ef">The entity or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The object or null.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public static WorkInfo GetWorkInfo(EfWork ef, BiblioDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (ef == null) return null;
            WorkInfo info = new WorkInfo
            {
                Id = ef.Id,
                Key = ef.Key,
                Type = ef.Type?.Name,
                Title = ef.Title,
                Language = ef.Language,
                Edition = ef.Edition,
                YearPub = ef.YearPub,
                PlacePub = ef.PlacePub,
                FirstPage = ef.FirstPage,
                LastPage = ef.LastPage
            };

            // authors
            if (ef.AuthorWorks?.Count > 0)
            {
                info.Authors = (from aw in ef.AuthorWorks
                                select new WorkAuthor
                                {
                                    First = aw.Author?.First,
                                    Last = aw.Author?.Last,
                                    Role = aw.Role
                                }).ToList();
            }

            // keywords
            if (ef.KeywordWorks?.Count > 0)
            {
                info.Keywords = (from kw in ef.KeywordWorks
                                 select new Keyword
                                 {
                                     Language = kw.Keyword?.Language,
                                     Value = kw.Keyword?.Value
                                 }).ToList();
            }

            // container
            if (ef.Container != null)
                info.Container = GetWorkInfo(ef.Container, context);

            return info;
        }

        /// <summary>
        /// Gets the container object corresponding to the specified container
        /// entity.
        /// </summary>
        /// <param name="ef">The entity or null.</param>
        /// <returns>The container or null.</returns>
        public static Container GetContainer(EfContainer ef)
        {
            if (ef == null) return null;

            Container container = new Container
            {
                Id = ef.Id,
                Key = ef.Key,
                Type = ef.Type?.Id,
                Title = ef.Title,
                Language = ef.Language,
                Edition = ef.Edition,
                Publisher = ef.Publisher,
                YearPub = ef.YearPub,
                PlacePub = ef.PlacePub,
                Location = ef.Location,
                AccessDate = ef.AccessDate,
                Note = ef.Note,
                Number = ef.Number
            };

            // authors
            if (ef.AuthorContainers?.Count > 0)
            {
                container.Authors.AddRange(from ac in ef.AuthorContainers
                                           select new WorkAuthor
                                           {
                                               Last = ac.Author.Last,
                                               First = ac.Author.First,
                                               Role = ac.Role
                                           });
            }

            // keywords
            if (ef.KeywordContainers?.Count > 0)
            {
                container.Keywords.AddRange(from ak in ef.KeywordContainers
                                            select GetKeyword(ak.Keyword));
            }

            return container;
        }

        /// <summary>
        /// Gets the work corresponding to the specified work entity.
        /// </summary>
        /// <param name="ef">The entity or null.</param>
        /// <returns>The work.</returns>
        public static Work GetWork(EfWork ef)
        {
            if (ef == null) return null;

            Work work = new Work
            {
                Id = ef.Id,
                Type = ef.Type?.Name,
                Title = ef.Title,
                Language = ef.Language,
                Container = GetContainer(ef.Container),
                Edition = ef.Edition,
                Publisher = ef.Publisher,
                YearPub = ef.YearPub,
                PlacePub = ef.PlacePub,
                Location = ef.Location,
                AccessDate = ef.AccessDate,
                FirstPage = ef.FirstPage,
                LastPage = ef.LastPage,
                Key = ef.Key,
                Note = ef.Note,
            };

            // authors
            if (ef.AuthorWorks?.Count > 0)
            {
                work.Authors.AddRange(from ac in ef.AuthorWorks
                                      select new WorkAuthor
                                      {
                                          Last = ac.Author.Last,
                                          First = ac.Author.First,
                                          Role = ac.Role
                                      });
            }

            // keywords
            if (ef.KeywordWorks?.Count > 0)
            {
                work.Keywords.AddRange(from aw in ef.KeywordWorks
                                       select GetKeyword(aw.Keyword));
            }

            return work;
        }
        #endregion

        #region POCO to Entity
        /// <summary>
        /// Gets the work entity corresponding to the specified type.
        /// </summary>
        /// <param name="type">The type or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The entity or null.</returns>
        public static EfWorkType GetEfWorkType(WorkType type,
            BiblioDbContext context)
        {
            if (type == null) return null;

            EfWorkType ef = type.Id != null
                ? context.WorkTypes.Find(type.Id) : null;
            if (ef == null)
            {
                ef = new EfWorkType
                {
                    Id = type.Id
                };
                context.WorkTypes.Add(ef);
            }
            ef.Name = type.Name;

            return ef;
        }

        /// <summary>
        /// Gets the author entity corresponding to the specified author.
        /// </summary>
        /// <param name="author">The author or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The entity or null.</returns>
        public static EfAuthor GetEfAuthor(Author author, BiblioDbContext context)
        {
            if (author == null) return null;

            EfAuthor ef = author.Id != Guid.Empty
                ? context.Authors.Find(author.Id) : null;
            if (ef == null)
            {
                if (author.Last == null) return null;
                ef = new EfAuthor
                {
                    Id = Guid.NewGuid()
                };
                context.Authors.Add(ef);
            }

            if (author.Last != null)
            {
                ef.First = author.First;
                ef.Last = author.Last;
                ef.Lastx = StandardFilter.Apply(author.Last, true);
                ef.Suffix = author.Suffix;
            }

            return ef;
        }

        private static void AddAuthors(IList<WorkAuthor> authors,
            EfContainer container,
            BiblioDbContext context)
        {
            // remove any author from the target work
            var old = context.AuthorContainers
                .Where(ac => ac.ContainerId == container.Id);
            context.AuthorContainers.RemoveRange(old);

            // add back the received authors
            container.AuthorContainers = new List<EfAuthorContainer>();

            foreach (WorkAuthor author in authors)
            {
                // find the author unless new
                EfAuthor efa = author.Id != Guid.Empty
                    ? context.Authors.Find(author.Id)
                    : null;

                if (efa == null)
                {
                    // if an existing author was required and we did not find
                    // him, ignore (defensive)
                    if (author.Last == null) continue;

                    // else we have a new author, add it
                    efa = new EfAuthor();
                    context.Authors.Add(efa);
                }

                // update the author if data are available
                // (we can consider an author empty if its Last property is empty)
                if (author.Last != null)
                {
                    efa.First = author.First;
                    efa.Last = author.Last;
                    efa.Lastx = StandardFilter.Apply(author.Last, true);
                }

                // add the link
                container.AuthorContainers.Add(new EfAuthorContainer
                {
                    Author = efa,
                    Role = author.Role,
                    Container = container
                });
            }
        }

        private static void AddKeywords(IList<Keyword> keywords,
            EfContainer container, BiblioDbContext context)
        {
            // remove any keyword from the target work
            var old = context.KeywordWorks.Where(kw => kw.WorkId == container.Id);
            context.KeywordWorks.RemoveRange(old);

            // add back the received keywords to it
            container.KeywordContainers = new List<EfKeywordContainer>();

            foreach (Keyword keyword in keywords)
            {
                EfKeyword efk = context.Keywords.FirstOrDefault(k =>
                    k.Value == keyword.Value && k.Language == keyword.Language);
                if (efk == null)
                {
                    efk = new EfKeyword();
                    context.Keywords.Add(efk);
                }
                efk.Language = keyword.Language;
                efk.Value = keyword.Value;
                efk.Valuex = StandardFilter.Apply(keyword.Value, true);

                container.KeywordContainers.Add(new EfKeywordContainer
                {
                    Keyword = efk,
                    Container = container
                });
            }
        }

        /// <summary>
        /// Gets the container entity corresponding to the specified container.
        /// </summary>
        /// <param name="container">The container or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The entity or null.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public static EfContainer GetEfContainer(Container container,
            BiblioDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (container == null) return null;

            // find the container unless new
            EfContainer ef = container.Id != Guid.Empty
                ? context.Containers.Find(container.Id) : null;

            // if new or not found, add it with a new ID
            if (ef == null)
            {
                if (container.Title == null) return null;
                ef = new EfContainer
                {
                    Id = Guid.NewGuid()
                };
                context.Containers.Add(ef);
            }

            // update the container unless empty
            if (container.Title != null)
            {
                ef.Type = context.WorkTypes.FirstOrDefault(t => t.Id == container.Type);
                // if the type does not exist, add it -- this is defensive,
                // and should not happen, as types are supposed to be a predefined set
                if (ef.Type == null)
                {
                    ef.Type = new EfWorkType
                    {
                        Id = container.Type,
                        Name = container.Type    // name=ID, better than nothing
                    };
                }
                ef.Title = container.Title;
                ef.Titlex = StandardFilter.Apply(container.Title, true);
                ef.Language = container.Language;
                ef.Edition = container.Edition;
                ef.Publisher = container.Publisher;
                ef.YearPub = container.YearPub;
                ef.PlacePub = container.PlacePub;
                ef.Location = container.Location;
                ef.AccessDate = container.AccessDate;
                ef.Number = container.Number;
                ef.Note = container.Note;

                // authors
                if (container.Authors?.Count > 0)
                    AddAuthors(container.Authors, ef, context);

                // keywords
                if (container.Keywords?.Count > 0)
                    AddKeywords(container.Keywords, ef, context);

                ef.Key = container.Key?.StartsWith(MAN_KEY_PREFIX) ?? false
                    ? container.Key : WorkKeyBuilder.Build(GetContainer(ef));

                // add key suffix if required and possible
                if (!container.Key.StartsWith(MAN_KEY_PREFIX))
                {
                    var existing = context.Works.FirstOrDefault(w => w.Key == ef.Key);
                    if (existing != null)
                    {
                        Match m = Regex.Match(existing.Key, @"\d+([a-z])?$");
                        if (m.Success)
                        {
                            char c = m.Groups[1].Value[0];
                            if (c < 'z') c++;
                            ef.Key += c;
                        }
                    }
                }
            }

            return ef;
        }

        private static void AddAuthors(IList<WorkAuthor> authors, EfWork work,
            BiblioDbContext context)
        {
            // remove any author from the target work
            var old = context.AuthorWorks.Where(aw => aw.WorkId == work.Id);
            context.AuthorWorks.RemoveRange(old);

            // add back the received authors
            work.AuthorWorks = new List<EfAuthorWork>();

            foreach (WorkAuthor author in authors)
            {
                // find the author unless new
                EfAuthor efa = author.Id != Guid.Empty
                    ? context.Authors.Find(author.Id)
                    : null;

                if (efa == null)
                {
                    // if an existing author was required and we did not find
                    // him, ignore (defensive)
                    if (author.Last == null) continue;

                    // else we have a new author, add it
                    efa = new EfAuthor();
                    context.Authors.Add(efa);
                }

                // update the author if data are available
                // (we can consider an author empty if its Last property is empty)
                if (author.Last != null)
                {
                    efa.First = author.First;
                    efa.Last = author.Last;
                    efa.Lastx = StandardFilter.Apply(author.Last, true);
                }

                // add the link
                work.AuthorWorks.Add(new EfAuthorWork
                {
                    Author = efa,
                    Role = author.Role,
                    Work = work
                });
            }
        }

        private static void AddKeywords(IList<Keyword> keywords, EfWork work,
            BiblioDbContext context)
        {
            // remove any keyword from the target work
            var old = context.KeywordWorks.Where(kw => kw.WorkId == work.Id);
            context.KeywordWorks.RemoveRange(old);

            // add back the received keywords to it
            work.KeywordWorks = new List<EfKeywordWork>();

            foreach (Keyword keyword in keywords)
            {
                EfKeyword efk = context.Keywords.FirstOrDefault(k =>
                    k.Value == keyword.Value && k.Language == keyword.Language);
                if (efk == null)
                {
                    efk = new EfKeyword();
                    context.Keywords.Add(efk);
                }
                efk.Language = keyword.Language;
                efk.Value = keyword.Value;
                efk.Valuex = StandardFilter.Apply(keyword.Value, true);

                work.KeywordWorks.Add(new EfKeywordWork
                {
                    Keyword = efk,
                    Work = work
                });
            }
        }

        /// <summary>
        /// Gets the work entity corresponding to the specified work.
        /// </summary>
        /// <param name="work">The work or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The entity or null.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public static EfWork GetEfWork(Work work, BiblioDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (work == null) return null;

            // find the work unless new
            EfWork ef = work.Id != Guid.Empty ? context.Works.Find(work.Id) : null;

            // if new or not found, add it with a new ID
            if (ef == null)
            {
                ef = new EfWork
                {
                    Id = Guid.NewGuid()
                };
                context.Works.Add(ef);
            }

            // update the work
            ef.Type = context.WorkTypes.FirstOrDefault(t => t.Id == work.Type);
            // if the type does not exist, add it -- this is defensive,
            // and should not happen, as types are supposed to be a predefined set
            if (ef.Type == null)
            {
                ef.Type = new EfWorkType
                {
                    Id = work.Type,
                    Name = work.Type    // name=ID, better than nothing
                };
            }
            ef.Title = work.Title;
            ef.Titlex = StandardFilter.Apply(work.Title, true);
            ef.Language = work.Language;
            ef.Container = GetEfContainer(work.Container, context);
            ef.Edition = work.Edition;
            ef.Publisher = work.Publisher;
            ef.YearPub = work.YearPub;
            ef.PlacePub = work.PlacePub;
            ef.Location = work.Location;
            ef.AccessDate = work.AccessDate;
            ef.FirstPage = work.FirstPage;
            ef.LastPage = work.LastPage;
            ef.Note = work.Note;

            // authors
            if (work.Authors?.Count > 0)
                AddAuthors(work.Authors, ef, context);

            // keywords
            if (work.Keywords?.Count > 0)
                AddKeywords(work.Keywords, ef, context);

            ef.Key = work.Key?.StartsWith(MAN_KEY_PREFIX) ?? false
                ? work.Key : WorkKeyBuilder.Build(GetWork(ef));

            // add key suffix if required and possible
            if (!work.Key.StartsWith(MAN_KEY_PREFIX))
            {
                var existing = context.Works.FirstOrDefault(w => w.Key == ef.Key);
                if (existing != null)
                {
                    Match m = Regex.Match(existing.Key, @"\d+([a-z])?$");
                    if (m.Success)
                    {
                        char c = m.Groups[1].Value[0];
                        if (c < 'z') c++;
                        ef.Key += c;
                    }
                }
            }

            return ef;
        }

        /// <summary>
        /// Gets the entity keyword corresponding to the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The entity or null.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public static EfKeyword GetEfKeyword(Keyword keyword, BiblioDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (keyword == null) return null;
            EfKeyword ef = context.Keywords.FirstOrDefault(
                k => k.Language == keyword.Language
                && k.Value == keyword.Value);
            if (ef == null)
            {
                ef = new EfKeyword();
                context.Keywords.Add(ef);
            }
            ef.Language = keyword.Language;
            ef.Value = keyword.Value;
            ef.Valuex = StandardFilter.Apply(keyword.Value, true);

            return ef;
        }
        #endregion
    }
}
