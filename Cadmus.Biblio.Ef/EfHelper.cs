using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Cadmus.Biblio.Ef
{
    public static class EfHelper
    {
        public static Keyword GetKeyword(EfKeyword ef)
        {
            if (ef == null) return null;
            return new Keyword
            {
                Language = ef.Language,
                Value = ef.Value
            };
        }

        public static Author GetAuthor(EfAuthor ef)
        {
            if (ef == null) return null;

            return new Author
            {
                First = ef.First,
                Last = ef.Last
            };
        }

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
                Number = ef.Number,
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
                                select new Author
                                {
                                    First = aw.Author?.First,
                                    Last = aw.Author?.Last,
                                    Role = aw.Role
                                }).ToList();
            }

            if (ef.ContributorWorks?.Count > 0)
            {
                work.Authors = (from aw in ef.ContributorWorks
                                select new Author
                                {
                                    First = aw.Author?.First,
                                    Last = aw.Author?.Last,
                                    Role = aw.Role
                                }).ToList();
            }

            if (ef.Keywords?.Count > 0)
            {
                work.Keywords = (from k in ef.Keywords
                                 select GetKeyword(k)).ToList();
            }

            return work;
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

        private static void AddContributors(IList<Author> authors, EfWork work,
            BiblioDbContext context)
        {
            // remove any contributor from the target work
            var old = context.ContributorWorks.Where(cw => cw.WorkId == work.Id);
            context.ContributorWorks.RemoveRange(old);

            // add the new contributors
            work.ContributorWorks = new List<EfContributorWork>();

            foreach (var author in authors)
            {
                var efa = context.Authors.FirstOrDefault(a =>
                    a.First == author.First && a.Last == author.Last);

                if (efa != null)
                {
                    work.ContributorWorks.Add(new EfContributorWork
                    {
                        Author = efa,
                        Work = work,
                        Role = author.Role
                    });
                }
                else
                {
                    work.ContributorWorks.Add(new EfContributorWork
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
            var old = context.Keywords.Where(k => k.WorkId == work.Id);
            context.Keywords.RemoveRange(old);

            // add the new contributors
            work.Keywords = new List<EfKeyword>();

            foreach (var keyword in keywords)
            {
                work.Keywords.Add(new EfKeyword
                {
                    Work = work,
                    Language = keyword.Language,
                    Value = keyword.Value,
                    Valuex = StandardFilter.Apply(keyword.Value, true),
                });
            }
        }

        public static EfWork GetWork(Work work, BiblioDbContext context)
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
