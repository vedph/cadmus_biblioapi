using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cadmus.Biblio.Ef
{
    public static class EfHelper
    {
        /// <summary>
        /// Gets the keyword corresponding to the specified EF keyword.
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
                First = ef.First,
                Last = ef.Last,
                Suffix = ef.Suffix
            };
        }

        /// <summary>
        /// Gets the work corresponding to the specified EF work.
        /// </summary>
        /// <param name="ef">The EF work or null.</param>
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
                Container = ef.Container,
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

            if (ef.AuthorWorks?.Count > 0)
            {
                work.Authors = (from aw in ef.AuthorWorks
                                select new WorkAuthor
                                {
                                    First = aw.Author?.First,
                                    Last = aw.Author?.Last,
                                    Role = aw.Role
                                }).ToList();
            }

            if (ef.KeywordWorks?.Count > 0)
            {
                work.Keywords = (from kw in ef.KeywordWorks
                                 select new Keyword
                                 {
                                     Language = kw.Keyword?.Language,
                                     Value = kw.Keyword?.Value
                                 }).ToList();
            }

            return work;
        }

        public static void UpdateAuthors(Work source, EfWork target,
            BiblioDbContext context)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            // delete no more existing authors
            // TODO

            // update existing authors and insert new authors
            // TODO
        }

        private static void AddAuthors(IList<Author> authors, EfWork work,
            BiblioDbContext context)
        {
            // remove any author from the target work
            var old = context.AuthorWorks.Where(aw => aw.WorkId == work.Id);
            context.AuthorWorks.RemoveRange(old);

            // add the new authors
            work.AuthorWorks = new List<EfAuthorWork>();

            foreach (var author in authors)
            {
                var efa = context.Authors.FirstOrDefault(a =>
                    a.First == author.First && a.Last == author.Last);

                if (efa != null)
                {
                    work.AuthorWorks.Add(new EfAuthorWork
                    {
                        Author = efa,
                        Work = work,
                        Role = author.Role
                    });
                }
                else
                {
                    work.AuthorWorks.Add(new EfAuthorWork
                    {
                        Author = new EfAuthor
                        {
                            First = author.First,
                            Last = author.Last,
                            Lastx = StandardFilter.Apply(author.Last, true)
                        },
                        Work = work,
                        Role = author.Role
                    });
                }
            }
        }

        private static void AddKeywords(IList<Keyword> keywords, EfWork work,
            BiblioDbContext context)
        {
            // remove any keyword from the target work
            var old = context.KeywordWorks.Where(kw => kw.WorkId == work.Id);
            context.KeywordWorks.RemoveRange(old);

            // add the new keywords
            work.KeywordWorks = new List<EfKeywordWork>();

            foreach (var keyword in keywords)
            {
                var efk = context.Keywords.FirstOrDefault(k =>
                    k.Value == keyword.Value && k.Language == keyword.Language);

                if (efk != null)
                {
                    work.KeywordWorks.Add(new EfKeywordWork
                    {
                        Keyword = efk,
                        Work = work
                    });
                }
                else
                {
                    work.KeywordWorks.Add(new EfKeywordWork
                    {
                        Keyword = new EfKeyword
                        {
                            Language = keyword.Language,
                            Value = keyword.Value,
                            Valuex = StandardFilter.Apply(keyword.Value, true)
                        },
                        Work = work
                    });
                }
            }
        }

        /// <summary>
        /// Gets a new EF work entity from the specified work.
        /// </summary>
        /// <param name="work">The work or null.</param>
        /// <param name="context">The context.</param>
        /// <returns>The new EF work entity or null.</returns>
        /// <exception cref="ArgumentNullException">context</exception>
        public static EfWork GetEfWork(Work work, BiblioDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (work == null) return null;

            EfWork ef = new EfWork
            {
                Id = work.Id,
                Type = context.Types.FirstOrDefault(t => t.Name == work.Type),
                Title = work.Title,
                Titlex = StandardFilter.Apply(work.Title, true),
                Language = work.Language,
                Container = work.Container,
                Containerx = StandardFilter.Apply(work.Container, true),
                Edition = work.Edition,
                Number = work.Number,
                Publisher = work.Publisher,
                YearPub = work.YearPub,
                PlacePub = work.PlacePub,
                Location = work.Location,
                AccessDate = work.AccessDate,
                FirstPage = work.FirstPage,
                LastPage = work.LastPage,
                Note = work.Note,
                Key = WorkKeyBuilder.Build(work)
            };

            // add key suffix if required and possible
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

            // authors
            if (work.Authors?.Count > 0)
                AddAuthors(work.Authors, ef, context);

            // contributors
            if (work.Contributors?.Count > 0)
                AddContributors(work.Contributors, ef, context);

            // keywords
            if (work.Keywords?.Count > 0)
                AddKeywords(work.Keywords, ef, context);

            return ef;
        }
    }
}
