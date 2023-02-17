using Cadmus.Biblio.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// Helper class for repositories.
/// </summary>
public static class EfHelper
{
    /// <summary>
    /// Gets the MySql schema for the bibliographic database.
    /// </summary>
    /// <returns>SQL code.</returns>
    public static string GetSchema()
    {
        using StreamReader reader = new(
            Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(
                "Cadmus.Biblio.Ef.Assets.cadmus-biblio.sql")!);
        return reader.ReadToEnd();
    }

    #region Entity to POCO        
    /// <summary>
    /// Gets the work's type corresponding to the specified work type entity.
    /// </summary>
    /// <param name="ef">The entity or null.</param>
    /// <returns>The work type or null.</returns>
    public static WorkType? GetWorkType(EfWorkType? ef)
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
    public static Keyword? GetKeyword(EfKeyword? ef)
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
    public static Author? GetAuthor(EfAuthor? ef)
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
    public static WorkInfo? GetWorkInfo(EfContainer? ef, BiblioDbContext? context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (ef == null) return null;
        WorkInfo info = new()
        {
            IsContainer = true,
            Id = ef.Id,
            Key = ef.Key,
            Type = ef.Type?.Id,
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
                                Id = aw.AuthorId,
                                First = aw.Author?.First,
                                Last = aw.Author?.Last,
                                Role = aw.Role,
                                Ordinal = aw.Ordinal
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
    public static WorkInfo? GetWorkInfo(EfWork? ef, BiblioDbContext? context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (ef == null) return null;
        WorkInfo info = new()
        {
            IsContainer = false,
            Id = ef.Id,
            Key = ef.Key,
            Type = ef.Type?.Id,
            Title = ef.Title,
            Language = ef.Language,
            Edition = ef.Edition,
            YearPub = ef.YearPub,
            PlacePub = ef.PlacePub,
            Number = ef.Number,
            FirstPage = ef.FirstPage,
            LastPage = ef.LastPage
        };

        // authors
        if (ef.AuthorWorks?.Count > 0)
        {
            info.Authors = (from aw in ef.AuthorWorks
                            select new WorkAuthor
                            {
                                Id = aw.AuthorId,
                                First = aw.Author?.First,
                                Last = aw.Author?.Last,
                                Role = aw.Role,
                                Ordinal = aw.Ordinal
                            }).ToList();
        }

        // keywords
        if (ef.KeywordWorks?.Count > 0)
        {
            info.Keywords = (from kw in ef.KeywordWorks
                             select new Keyword
                             {
                                 Language = kw.Keyword?.Language!,
                                 Value = kw.Keyword?.Value!
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
    public static Container? GetContainer(EfContainer? ef)
    {
        if (ef == null) return null;

        Container container = new()
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
                                           Last = ac.Author?.Last,
                                           First = ac.Author?.First,
                                           Role = ac.Role,
                                           Ordinal = ac.Ordinal
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
    public static Work? GetWork(EfWork? ef)
    {
        if (ef == null) return null;

        Work work = new()
        {
            Id = ef.Id,
            Type = ef.Type?.Id,
            Title = ef.Title,
            Language = ef.Language,
            Container = GetContainer(ef.Container),
            Edition = ef.Edition,
            Publisher = ef.Publisher,
            YearPub = ef.YearPub,
            PlacePub = ef.PlacePub,
            Location = ef.Location,
            AccessDate = ef.AccessDate,
            Number = ef.Number,
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
                                      Id = ac.AuthorId,
                                      Last = ac.Author?.Last,
                                      First = ac.Author?.First,
                                      Role = ac.Role,
                                      Ordinal = ac.Ordinal
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
    /// Ensures that the entity corresponding to the specified type exists.
    /// </summary>
    /// <param name="type">The type or null.</param>
    /// <param name="context">The context.</param>
    /// <returns>The entity (or null for a null type).</returns>
    public static EfWorkType? EnsureEfWorkType(WorkType? type,
        BiblioDbContext context)
    {
        if (type == null) return null;

        EfWorkType? ef = type.Id != null
            ? context.WorkTypes.Find(type.Id) : null;
        if (ef == null)
        {
            ef = new EfWorkType
            {
                Id = type.Id!
            };
            context?.WorkTypes.Add(ef);
        }
        ef.Name = type.Name!;

        return ef;
    }

    /// <summary>
    /// Gets the author entity corresponding to the specified author.
    /// </summary>
    /// <param name="author">The author or null.</param>
    /// <param name="context">The context.</param>
    /// <returns>The entity or null.</returns>
    /// <exception cref="ArgumentNullException">context</exception>
    public static EfAuthor? GetEfAuthor(Author? author, BiblioDbContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (author == null) return null;

        EfAuthor? ef = author.Id != Guid.Empty
            ? context.Authors.Find(author.Id) : null;
        if (ef == null)
        {
            if (author.Last == null) return null;
            ef = new EfAuthor();
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
        // collect the authors to be assigned, adding the missing ones
        List<EfAuthorContainer> requested = new();
        foreach (WorkAuthor author in authors)
        {
            EfAuthor? efa = GetEfAuthorFor(author, context);
            if (efa == null) continue;
            requested.Add(new EfAuthorContainer
            {
                Author = efa,
                Container = container
            });
        } // for

        // remove all the no more requested authors
        if (container.AuthorContainers != null)
        {
            foreach (EfAuthorContainer ac in container.AuthorContainers)
            {
                if (requested.All(r => r.AuthorId != ac.AuthorId))
                    context.AuthorContainers.Remove(ac);
            }
        }
        else container.AuthorContainers = new List<EfAuthorContainer>();

        // add all those which are not yet present
        foreach (EfAuthorContainer ac in requested)
        {
            if (container.AuthorContainers.All(
                r => r.AuthorId != ac.AuthorId))
            {
                container.AuthorContainers.Add(ac);
            }
        }
    }

    private static void AddKeywords(IList<Keyword> keywords,
        EfContainer container, BiblioDbContext context)
    {
        // collect the keywords to be assigned, adding the missing ones
        List<EfKeywordContainer> requested = new();
        foreach (Keyword keyword in keywords)
        {
            // find the keyword by its content, as we have no ID
            EfKeyword? efk = context.Keywords.FirstOrDefault(k =>
                k.Value == keyword.Value && k.Language == keyword.Language);

            // if not found, add it
            if (efk == null)
            {
                efk = new EfKeyword
                {
                    Language = keyword.Language!,
                    Value = keyword.Value!,
                    Valuex = StandardFilter.Apply(keyword.Value!, true)
                };
                context.Keywords.Add(efk);
            }

            requested.Add(new EfKeywordContainer
            {
                Keyword = efk,
                Container = container
            });
        }

        // remove all the keywords which are no more requested
        if (container.KeywordContainers != null)
        {
            foreach (EfKeywordContainer kc in container.KeywordContainers)
            {
                if (requested.All(r => r.KeywordId != kc.KeywordId))
                    context.KeywordContainers.Remove(kc);
            }
        }
        else container.KeywordContainers = new List<EfKeywordContainer>();

        // add all those which are not yet present
        foreach (EfKeywordContainer kc in requested)
        {
            if (container.KeywordContainers.All(
                r => r.KeywordId != kc.KeywordId))
            {
                container.KeywordContainers.Add(kc);
            }
        }
    }

    private static EfWorkType GetOrCreateWorkType(string? id, string? name,
        BiblioDbContext context)
    {
        // force a valid type
        id ??= "-";
        name ??= id;

        // get it
        EfWorkType? ef = context.WorkTypes.Find(id);
        if (ef == null)
        {
            ef = new EfWorkType
            {
                Id = id,
                Name = name
            };
            context.WorkTypes.Add(ef);
        }
        else if (name is not null)
        {
            ef.Name = name;
        }

        return ef;
    }

    /// <summary>
    /// Gets the container entity corresponding to the specified container.
    /// </summary>
    /// <param name="container">The container or null.</param>
    /// <param name="context">The context.</param>
    /// <returns>The entity or null.</returns>
    /// <exception cref="ArgumentNullException">context</exception>
    public static EfContainer? GetEfContainer(Container? container,
        BiblioDbContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));
        if (container == null) return null;

        // get the container unless it's new
        EfContainer? ef = container.Id != Guid.Empty
            ? context.Containers
                .Include(c => c.AuthorContainers)
                .Include(c => c.KeywordContainers)
                .FirstOrDefault(c => c.Id == container.Id)
            : null;

        // if new or not found, add it with a new ID
        if (ef == null)
        {
            if (container.Title == null) return null;
            ef = new EfContainer();
            context.Containers.Add(ef);
        }

        // update the container unless empty
        if (container.Title != null)
        {
            ef.Type = GetOrCreateWorkType(container.Type, null, context);
            ef.Title = container.Title;
            ef.Titlex = StandardFilter.Apply(container.Title, true);
            ef.Language = container.Language!;
            ef.Edition = container.Edition;
            ef.Publisher = container.Publisher!;
            ef.YearPub = container.YearPub;
            ef.PlacePub = container.PlacePub!;
            ef.Location = container.Location!;
            ef.AccessDate = container.AccessDate;
            ef.Number = container.Number!;
            ef.Note = container.Note!;

            // authors
            if (container.Authors?.Count > 0)
                AddAuthors(container.Authors, ef, context);

            // keywords
            if (container.Keywords?.Count > 0)
                AddKeywords(container.Keywords, ef, context);

            // key
            ef.Key = WorkKeyBuilder.PickKey(ef.Key!, container);
            // add key suffix if required and possible
            if (ef.Key?.StartsWith(WorkKeyBuilder.MAN_KEY_PREFIX) != true)
            {
                char c = GetSuffixForKey(ef.Key!, context);
                if (c != '\0') ef.Key += c;
            }
        }

        return ef;
    }

    private static EfAuthor? GetEfAuthorFor(WorkAuthor author,
        BiblioDbContext context)
    {
        // find the author
        EfAuthor? efa = author.Id != Guid.Empty
            ? context.Authors.Find(author.Id) : null;

        // if not found, add a new author
        if (efa == null)
        {
            // if an existing author was required but was not found,
            // just ignore him (defensive)
            if (author.Last == null) return null;

            // else we have a new author, add it
            efa = new EfAuthor
            {
                First = author.First,
                Last = author.Last,
                Suffix = author.Suffix
            };
            author.Id = efa.Id;         // update the received ID
            context.Authors.Add(efa);
        }
        else
        {
            // if found, supply data in the source author if empty
            if (author.Last == null)
            {
                author.First = efa.First;
                author.Last = efa.Last;
                author.Suffix = efa.Suffix;
            }
            // else update the target author
            else
            {
                efa.First = author.First;
                efa.Last = author.Last;
                efa.Suffix = author.Suffix;
            }
        }
        // update indexed last
        efa.Lastx = StandardFilter.Apply(efa.Last!, true);

        return efa;
    }

    private static void AddAuthors(IList<WorkAuthor> authors, EfWork work,
        BiblioDbContext context)
    {
        // collect the authors to be assigned, adding the missing ones
        List<EfAuthorWork> requested = new();
        foreach (WorkAuthor author in authors)
        {
            EfAuthor? efa = GetEfAuthorFor(author, context);
            if (efa == null) continue;
            requested.Add(new EfAuthorWork
            {
                Author = efa,
                AuthorId = efa.Id,
                Work = work,
                WorkId = work.Id,
            });
        }

        // remove all the no more requested authors
        if (work.AuthorWorks != null)
        {
            foreach (EfAuthorWork aw in work.AuthorWorks)
            {
                if (requested.All(r => r.AuthorId != aw.AuthorId))
                    context.AuthorWorks.Remove(aw);
            }
        }
        else
        {
            work.AuthorWorks = new List<EfAuthorWork>();
        }

        // add all those which are not yet present
        foreach (EfAuthorWork aw in requested)
        {
            if (work.AuthorWorks.All(
                r => r.AuthorId != aw.AuthorId))
            {
                work.AuthorWorks.Add(aw);
            }
        }
    }

    private static void AddKeywords(IList<Keyword> keywords, EfWork work,
        BiblioDbContext context)
    {
        // collect the keywords to be assigned, adding the missing ones
        List<EfKeywordWork> requested = new();
        foreach (Keyword keyword in keywords)
        {
            // find the keyword by its content, as we have no ID
            EfKeyword? efk = context.Keywords.FirstOrDefault(k =>
                k.Value == keyword.Value && k.Language == keyword.Language);

            // if not found, add it
            if (efk == null)
            {
                efk = new EfKeyword
                {
                    Language = keyword.Language!,
                    Value = keyword.Value!,
                    Valuex = StandardFilter.Apply(keyword.Value!, true)
                };
                context.Keywords.Add(efk);
            }

            requested.Add(new EfKeywordWork
            {
                Keyword = efk,
                Work = work
            });
        }

        // remove all the keywords which are no more requested
        if (work.KeywordWorks != null)
        {
            foreach (EfKeywordWork kw in work.KeywordWorks)
            {
                if (requested.All(r => r.KeywordId != kw.KeywordId))
                    context.KeywordWorks.Remove(kw);
            }
        }
        else work.KeywordWorks = new List<EfKeywordWork>();

        // add all those which are not yet present
        foreach (EfKeywordWork kw in requested)
        {
            if (work.KeywordWorks.All(
                r => r.KeywordId != kw.KeywordId))
            {
                work.KeywordWorks.Add(kw);
            }
        }
    }

    private static char GetSuffixForKey(string key, BiblioDbContext context)
    {
        var existing = context.Works.FirstOrDefault(w => w.Key == key);
        if (existing != null)
        {
            Match m = Regex.Match(existing.Key ?? "", @"\d+([a-z])?$");
            if (m.Success && m.Groups[1].Value.Length > 0)
            {
                char c = m.Groups[1].Value[0];
                if (c < 'z') c++;
                return c;
            }
        }
        return '\0';
    }

    /// <summary>
    /// Gets the work entity corresponding to the specified work.
    /// </summary>
    /// <param name="work">The work or null.</param>
    /// <param name="context">The context.</param>
    /// <returns>The entity or null.</returns>
    /// <exception cref="ArgumentNullException">context</exception>
    public static EfWork? GetEfWork(Work? work, BiblioDbContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (work == null) return null;

        // find the work unless new
        EfWork? ef = work.Id != Guid.Empty
            ? context.Works
                .Include(w => w.AuthorWorks)
                .Include(w => w.KeywordWorks)
                .FirstOrDefault(w => w.Id == work.Id)
            : null;

        // if new or not found, add it with a new ID
        if (ef == null)
        {
            ef = new EfWork();
            context.Works.Add(ef);
        }

        // update the work
        ef.Type = GetOrCreateWorkType(work.Type, null, context);
        ef.Title = work.Title!;
        ef.Titlex = StandardFilter.Apply(work.Title!, true);
        ef.Language = work.Language!;
        ef.Container = GetEfContainer(work.Container, context);
        ef.Edition = work.Edition;
        ef.Publisher = work.Publisher;
        ef.YearPub = work.YearPub;
        ef.PlacePub = work.PlacePub;
        ef.Location = work.Location;
        ef.AccessDate = work.AccessDate;
        ef.Number = work.Number;
        ef.FirstPage = work.FirstPage;
        ef.LastPage = work.LastPage;
        ef.Note = work.Note;

        // authors
        if (work.Authors?.Count > 0)
            AddAuthors(work.Authors, ef, context);

        // keywords
        if (work.Keywords?.Count > 0)
            AddKeywords(work.Keywords, ef, context);

        // key
        ef.Key = WorkKeyBuilder.PickKey(ef.Key!, work);
        // add key suffix if required and possible
        if (ef.Key?.StartsWith(WorkKeyBuilder.MAN_KEY_PREFIX) != true)
        {
            char c = GetSuffixForKey(ef.Key!, context);
            if (c != '\0') ef.Key += c;
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
    public static EfKeyword? GetEfKeyword(Keyword keyword, BiblioDbContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (keyword == null) return null;
        EfKeyword? ef = context.Keywords.FirstOrDefault(
            k => k.Language == keyword.Language
            && k.Value == keyword.Value);
        if (ef == null)
        {
            ef = new EfKeyword
            {
                Language = keyword.Language!,
                Value = keyword.Value!,
                Valuex = StandardFilter.Apply(keyword.Value!, true)
            };
            context.Keywords.Add(ef);
        }

        return ef;
    }
    #endregion
}
