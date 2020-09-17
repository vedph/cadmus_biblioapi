﻿using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets the work by its internal ID.
        /// </summary>
        /// <param name="id">The work's internal identifier.</param>
        /// <returns>
        /// Work or null if not found
        /// </returns>
        public Work GetWork(int id)
        {
            using (var db = GetContext())
            {
                EfWork work = db.Works
                    .AsNoTracking()
                    .Include(w => w.Type)
                    .Include(w => w.AuthorWorks)
                    .ThenInclude(w => w.Author)
                    .Include(w => w.ContributorWorks)
                    .ThenInclude(w => w.Author)
                    .FirstOrDefault(w => w.Id == id);
                return EfHelper.GetWork(work);
            }
        }

        private void PrepareFilter(WorkFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LastName))
                filter.LastName = StandardFilter.Apply(filter.LastName, true);

            if (!string.IsNullOrEmpty(filter.Title))
                filter.Title = StandardFilter.Apply(filter.Title, true);

            if (!string.IsNullOrEmpty(filter.Container))
                filter.Container = StandardFilter.Apply(filter.Container, true);
        }

        /// <summary>
        /// Gets the specified page of filtered works.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>
        /// The works page.
        /// </returns>
        /// <exception cref="ArgumentNullException">filter</exception>
        public DataPage<Work> GetWorks(WorkFilter filter)
        {
            if (filter == null) throw new ArgumentNullException(nameof(filter));

            PrepareFilter(filter);

            using (var db = GetContext())
            {
                IQueryable<EfWork> works = db.Works
                    .AsNoTracking()
                    .Include(w => w.Type)
                    .Include(w => w.AuthorWorks)
                    .ThenInclude(w => w.Author)
                    .Include(w => w.ContributorWorks)
                    .ThenInclude(w => w.Author);

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
                            || w.AuthorWorks.Any(aw => aw.Author.Lastx.Contains(filter.LastName))
                        // language
                        || string.IsNullOrEmpty(filter.Language)
                            || w.Language == filter.Language
                        // title
                        || string.IsNullOrEmpty(filter.Title)
                            || w.Titlex.Contains(filter.Title)
                        // container
                        || string.IsNullOrEmpty(filter.Container)
                            || w.Containerx.Contains(filter.Container)
                        // keyword
                        || string.IsNullOrEmpty(filter.Keyword)
                            || w.Keywords.Any(k => k.Valuex.Contains(filter.Keyword))
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
                    if (!string.IsNullOrEmpty(filter.Container))
                        works = works.Where(w => w.Containerx.Contains(filter.Container));

                    // keyword
                    if (!string.IsNullOrEmpty(filter.Keyword))
                    {
                        works = works.Where(w => w.Keywords.Any(
                            k => k.Valuex.Contains(filter.Keyword)));
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
                    .ThenBy(w => w.Key);
                works = works.Skip(filter.GetSkipCount()).Take(filter.PageSize);

                return new DataPage<Work>(filter.PageNumber, filter.PageSize,
                    tot, (from w in works select EfHelper.GetWork(w)).ToList());
            }
        }

        public string GetYearSuffix(string key)
        {
            throw new NotImplementedException();
        }

        public int AddWork(Work work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));

            using (var db = GetContext())
            {
                EfWork ef;

                // remove existing work
                if (work.Id > 0)
                {
                    ef = db.Works.Find(work.Id);
                    if (ef != null) db.Works.Remove(ef);
                }

                ef = EfHelper.GetWork(work, db);
                db.Works.Add(ef);

                db.SaveChanges();
                return ef.Id;
            }
        }

        /// <summary>
        /// Deletes the work with the specified internal ID.
        /// </summary>
        /// <param name="id">The work's internal identifier.</param>
        public void DeleteWork(int id)
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

        /// <summary>
        /// Gets the first <paramref name="count" /> type names including
        /// <paramref name="name" /> in their name, in alphabetical order.
        /// </summary>
        /// <param name="name">The part of the name to be matched. It can be
        /// null or empty when you want to match any type.</param>
        /// <param name="count">The maximum count of desired results, or
        /// 0 to get all the types.</param>
        /// <returns>
        /// Type names.
        /// </returns>
        public IList<string> GetTypes(string name, int count)
        {
            using (var db = GetContext())
            {
                var names = from t in db.Types
                            where name == null || t.Name == "" || t.Name.Contains(name)
                            orderby t.Name
                            select t.Name;
                return count > 0 ? names.Take(count).ToList() : names.ToList();
            }
        }

        /// <summary>
        /// Adds the type with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// The type's internal ID.
        /// </returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public int AddType(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            using (var db = GetContext())
            {
                EfWorkType type = db.Types.AsNoTracking()
                    .FirstOrDefault(t => t.Name == name);
                if (type != null) return type.Id;

                type = new EfWorkType
                {
                    Name = name
                };
                db.Types.Add(type);
                db.SaveChanges();
                return type.Id;
            }
        }

        /// <summary>
        /// Deletes the type with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void DeleteType(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            using (var db = GetContext())
            {
                EfWorkType type = db.Types.FirstOrDefault(t => t.Name == name);
                if (type == null) return;

                db.Types.Remove(type);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Gets the first <paramref name="count" /> authors including
        /// <paramref name="last" /> in their last name.
        /// </summary>
        /// <param name="last">The part of the last name to be matched.</param>
        /// <param name="count">The maximum count of desired results.</param>
        /// <returns>
        /// Authors.
        /// </returns>
        public IList<Author> GetAuthors(string last, int count)
        {
            if (last == null) throw new ArgumentNullException(nameof(last));
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));

            using (var db = GetContext())
            {
                var authors = (from a in db.Authors
                               where a.Lastx.Contains(last)
                               orderby a.Lastx, a.First
                               select a).Take(count).ToList();

                return authors.Select(EfHelper.GetAuthor).ToList();
            }
        }

        /// <summary>
        /// Prunes the authors by removing all the authors without any work.
        /// This can be used to shrink the database and remove unused authors
        /// from it, as authors get added when adding works.
        /// </summary>
        public void PruneAuthors()
        {
            using (var db = GetContext())
            {
                var authors = from a in db.Authors
                              where db.AuthorWorks.All(aw => aw.AuthorId != a.Id)
                                && db.ContributorWorks.All(cw => cw.AuthorId != a.Id)
                              select a;
                db.Authors.RemoveRange(authors);
                db.SaveChanges();
            }
        }
    }
}
