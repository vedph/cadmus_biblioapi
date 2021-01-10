﻿using Cadmus.Biblio.Core;
using Fusi.DbManager;
using Fusi.DbManager.MySql;
using Xunit;

namespace Cadmus.Biblio.Ef.Test
{
    // https://github.com/xunit/xunit/issues/1999

    [CollectionDefinition(nameof(NonParallelResourceCollection),
        DisableParallelization = true)]
    public class NonParallelResourceCollection { }

    [Collection(nameof(NonParallelResourceCollection))]
    public sealed class EfBiblioRepositoryTest
    {
        private const string CST = "Server=localhost;Database={0};Uid=root;Pwd=mysql";
        private const string DB = "cadmus-biblio-test";

        private readonly IDbManager _manager;

        public EfBiblioRepositoryTest()
        {
            _manager = new MySqlDbManager(string.Format(CST, DB));
        }

        private void ResetDatabase()
        {
            if (_manager.Exists(DB))
                _manager.ClearDatabase(DB);
            else
                _manager.CreateDatabase(DB, EfHelper.GetSchema(), null);
        }

        private static IBiblioRepository GetRepository()
        {
            return new EfBiblioRepository(
                string.Format(CST, "cadmus-biblio-test"), "mysql");
        }

        #region Work types
        [Fact]
        public void AddWorkType_NotExisting_Added()
        {
            ResetDatabase();
            var repository = GetRepository();
            WorkType type = new WorkType
            {
                Id = "book",
                Name = "Book"
            };

            repository.AddWorkType(type);

            WorkType type2 = repository.GetWorkType("book");
            Assert.NotNull(type2);
            Assert.Equal(type.Id, type2.Id);
            Assert.Equal(type.Name, type2.Name);
        }

        [Fact]
        public void AddWorkType_Existing_Updated()
        {
            ResetDatabase();
            var repository = GetRepository();
            WorkType type = new WorkType
            {
                Id = "book",
                Name = "Book"
            };
            repository.AddWorkType(type);

            type.Name = "Libro";
            repository.AddWorkType(type);

            WorkType type2 = repository.GetWorkType("book");
            Assert.NotNull(type2);
            Assert.Equal(type.Id, type2.Id);
            Assert.Equal(type.Name, type2.Name);
        }

        [Fact]
        public void DeleteWorkType_NotExisting_Nope()
        {
            ResetDatabase();
            var repository = GetRepository();
            WorkType type = new WorkType
            {
                Id = "book",
                Name = "Book"
            };
            repository.AddWorkType(type);

            repository.DeleteWorkType("not-existing");

            Assert.NotNull(repository.GetWorkType("book"));
        }

        [Fact]
        public void DeleteWorkType_Existing_Deleted()
        {
            ResetDatabase();
            var repository = GetRepository();
            WorkType type = new WorkType
            {
                Id = "book",
                Name = "Book"
            };
            repository.AddWorkType(type);

            repository.DeleteWorkType("book");

            Assert.Null(repository.GetWorkType("book"));
        }

        [Fact]
        public void GetWorkTypes_PagedUnfiltered_Ok()
        {
            ResetDatabase();
            var repository = GetRepository();
            repository.AddWorkType(new WorkType
            {
                Id = "book",
                Name = "Book"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "journal",
                Name = "Journal"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "procs",
                Name = "Proceedings"
            });

            var page = repository.GetWorkTypes(new WorkTypeFilter
            {
                PageNumber = 1,
                PageSize = 2
            });

            Assert.Equal(3, page.Total);
            Assert.Equal(2, page.Items.Count);

            WorkType type2 = page.Items[0];
            Assert.Equal("book", type2.Id);
            Assert.Equal("Book", type2.Name);

            type2 = page.Items[1];
            Assert.Equal("journal", type2.Id);
            Assert.Equal("Journal", type2.Name);
        }

        [Fact]
        public void GetWorkTypes_PagedFiltered_Ok()
        {
            ResetDatabase();
            var repository = GetRepository();
            repository.AddWorkType(new WorkType
            {
                Id = "book",
                Name = "Book"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "journal",
                Name = "Journal"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "procs",
                Name = "Proceedings"
            });

            var page = repository.GetWorkTypes(new WorkTypeFilter
            {
                PageNumber = 1,
                PageSize = 2,
                Name = "j"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);

            WorkType type2 = page.Items[0];
            Assert.Equal("journal", type2.Id);
            Assert.Equal("Journal", type2.Name);
        }

        [Fact]
        public void GetWorkTypes_UnpagedUnfiltered_Ok()
        {
            ResetDatabase();
            var repository = GetRepository();
            repository.AddWorkType(new WorkType
            {
                Id = "book",
                Name = "Book"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "journal",
                Name = "Journal"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "procs",
                Name = "Proceedings"
            });

            var page = repository.GetWorkTypes(new WorkTypeFilter
            {
                PageNumber = 1,
                PageSize = 0    // unpaged
            });

            Assert.Equal(3, page.Total);
            Assert.Equal(3, page.Items.Count);

            WorkType type2 = page.Items[0];
            Assert.Equal("book", type2.Id);
            Assert.Equal("Book", type2.Name);

            type2 = page.Items[1];
            Assert.Equal("journal", type2.Id);
            Assert.Equal("Journal", type2.Name);

            type2 = page.Items[2];
            Assert.Equal("procs", type2.Id);
            Assert.Equal("Proceedings", type2.Name);
        }

        [Fact]
        public void GetWorkTypes_UnpagedFiltered_Ok()
        {
            ResetDatabase();
            var repository = GetRepository();
            repository.AddWorkType(new WorkType
            {
                Id = "book",
                Name = "Book"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "journal",
                Name = "Journal"
            });
            repository.AddWorkType(new WorkType
            {
                Id = "procs",
                Name = "Proceedings"
            });

            var page = repository.GetWorkTypes(new WorkTypeFilter
            {
                PageNumber = 1,
                PageSize = 0,    // unpaged,
                Name = "j"
            });

            Assert.Equal(1, page.Total);
            Assert.Equal(1, page.Items.Count);

            WorkType type2 = page.Items[0];
            Assert.Equal("journal", type2.Id);
            Assert.Equal("Journal", type2.Name);
        }
        #endregion

        #region Keywords
        [Fact]
        public void AddKeyword_NotExisting_Added()
        {
            ResetDatabase();
            var repository = GetRepository();
            Keyword keyword = new Keyword
            {
                Language = "eng",
                Value = "test"
            };

            int id = repository.AddKeyword(keyword);

            Keyword keyword2 = repository.GetKeyword(id);
            Assert.NotNull(keyword2);
            Assert.Equal(keyword.Language, keyword2.Language);
            Assert.Equal(keyword.Value, keyword2.Value);
        }

        [Fact]
        public void AddKeyword_Existing_Nope()
        {
            ResetDatabase();
            var repository = GetRepository();
            Keyword keyword = new Keyword
            {
                Language = "eng",
                Value = "test"
            };
            int id = repository.AddKeyword(keyword);

            int id2 = repository.AddKeyword(keyword);

            Assert.Equal(id, id2);
        }

        [Fact]
        public void DeleteKeyword_NotExisting_Nope()
        {
            ResetDatabase();
            var repository = GetRepository();
            Keyword keyword = new Keyword
            {
                Language = "eng",
                Value = "test"
            };
            int id = repository.AddKeyword(keyword);

            repository.DeleteKeyword(123);
            
            Assert.NotNull(repository.GetKeyword(id));
        }

        [Fact]
        public void DeleteKeyword_Existing_Deleted()
        {
            // TODO
        }
        #endregion
    }
}
