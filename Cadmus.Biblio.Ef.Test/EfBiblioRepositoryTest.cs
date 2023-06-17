using Cadmus.Biblio.Core;
using Fusi.DbManager;
using Fusi.DbManager.PgSql;
using System;
using System.Collections.Generic;
using Xunit;

namespace Cadmus.Biblio.Ef.Test;

// https://github.com/xunit/xunit/issues/1999

[CollectionDefinition(nameof(NonParallelResourceCollection),
    DisableParallelization = true)]
public class NonParallelResourceCollection { }

[Collection(nameof(NonParallelResourceCollection))]
public sealed class EfBiblioRepositoryTest
{
    private const string CST = "Server=localhost;port=5432;Database={0};" +
        "User Id=postgres;Password=postgres;Include Error Detail=True";
    private const string DB = "cadmus-biblio-test";

    private readonly IDbManager _manager;

    public EfBiblioRepositoryTest()
    {
        _manager = new PgSqlDbManager(CST);
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
            string.Format(CST, "cadmus-biblio-test"), "pgsql");
    }

    #region Work types
    [Fact]
    public void AddWorkType_NotExisting_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        WorkType type = new()
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
        WorkType type = new()
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
        WorkType type = new()
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
        WorkType type = new()
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
        Keyword keyword = new()
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
        Keyword keyword = new()
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
        Keyword keyword = new()
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
        ResetDatabase();
        var repository = GetRepository();
        Keyword keyword = new()
        {
            Language = "eng",
            Value = "test"
        };
        int id = repository.AddKeyword(keyword);

        repository.DeleteKeyword(id);

        Assert.Null(repository.GetKeyword(id));
    }

    [Fact]
    public void PruneKeywords_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        int koid = repository.AddKeyword(new Keyword
        {
            Language = "eng",
            Value = "orphan"
        });
        Keyword kw = new()
        {
            Language = "eng",
            Value = "work"
        };
        int kwid = repository.AddKeyword(kw);
        Keyword kc = new()
        {
            Language = "eng",
            Value = "container"
        };
        int kcid = repository.AddKeyword(kc);

        // work with kw
        repository.AddWork(new Work
        {
            Key = "Rossi 1963",
            Title = "Title",
            Language = "ita",
            YearPub = 1963,
            Keywords = new List<Keyword>(new[] { kw })
        });
        // container with kc
        repository.AddContainer(new Container
        {
            Key = "Bianchi 1970",
            Title = "Title",
            Language = "ita",
            YearPub = 1970,
            Keywords = new List<Keyword>(new[] { kc })
        });

        repository.PruneKeywords();

        Assert.NotNull(repository.GetKeyword(kwid));
        Assert.NotNull(repository.GetKeyword(kcid));
        Assert.Null(repository.GetKeyword(koid));
    }

    private static IList<Keyword> GetSampleKeywords()
    {
        return new List<Keyword>(new[]
        {
            new Keyword
            {
                Language = "eng",
                Value = "green"
            },
            new Keyword
            {
                Language = "eng",
                Value = "red"
            },
            new Keyword
            {
                Language = "ita",
                Value = "rosso"
            }
        });
    }

    [Fact]
    public void GetKeywords_Unfiltered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Keyword keyword in GetSampleKeywords())
            repository.AddKeyword(keyword);

        var page = repository.GetKeywords(new KeywordFilter
        {
            PageNumber = 1,
            PageSize = 2,
        });

        Assert.Equal(3, page.Total);
        Assert.Equal(2, page.Items.Count);

        Assert.Equal("green", page.Items[0].Value);
        Assert.Equal("red", page.Items[1].Value);
    }

    [Fact]
    public void GetKeywords_ByLanguage_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Keyword keyword in GetSampleKeywords())
            repository.AddKeyword(keyword);

        var page = repository.GetKeywords(new KeywordFilter
        {
            PageNumber = 1,
            PageSize = 2,
            Language = "ita"
        });

        Assert.Equal(1, page.Total);
        Assert.Equal(1, page.Items.Count);

        Assert.Equal("rosso", page.Items[0].Value);
    }

    [Fact]
    public void GetKeywords_ByValue_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Keyword keyword in GetSampleKeywords())
            repository.AddKeyword(keyword);

        var page = repository.GetKeywords(new KeywordFilter
        {
            PageNumber = 1,
            PageSize = 2,
            Value = "re"
        });

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);

        Assert.Equal("green", page.Items[0].Value);
        Assert.Equal("red", page.Items[1].Value);
    }

    [Fact]
    public void GetKeywords_ByLanguageAndValue_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Keyword keyword in GetSampleKeywords())
            repository.AddKeyword(keyword);

        var page = repository.GetKeywords(new KeywordFilter
        {
            PageNumber = 1,
            PageSize = 2,
            Language = "ita",
            Value = "re"
        });

        Assert.Equal(0, page.Total);
        Assert.Equal(0, page.Items.Count);
    }
    #endregion

    #region Authors
    [Fact]
    public void AddAuthor_NotExisting_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
            Suffix = "jr."
        };

        repository.AddAuthor(author);

        Assert.NotEqual(Guid.Empty, author.Id);
        Author author2 = repository.GetAuthor(author.Id);
        Assert.NotNull(author2);
        Assert.Equal(author.First, author2.First);
        Assert.Equal(author.Last, author2.Last);
        Assert.Equal(author.Suffix, author2.Suffix);
    }

    [Fact]
    public void AddAuthor_Existing_Updated()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
        };
        repository.AddAuthor(author);
        Guid id = author.Id;

        author.First = "Johnny";
        author.Last = "Doherty";
        author.Suffix = "jr.";
        repository.AddAuthor(author);

        Assert.Equal(id, author.Id);
        Author author2 = repository.GetAuthor(author.Id);
        Assert.NotNull(author2);
        Assert.Equal(author.First, author2.First);
        Assert.Equal(author.Last, author2.Last);
        Assert.Equal(author.Suffix, author2.Suffix);
    }

    [Fact]
    public void DeleteAuthor_NotExisting_Nope()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
            Suffix = "jr."
        };
        repository.AddAuthor(author);

        repository.DeleteAuthor(Guid.NewGuid());

        Assert.NotNull(repository.GetAuthor(author.Id));
    }

    [Fact]
    public void DeleteAuthor_Existing_Deleted()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
            Suffix = "jr."
        };
        repository.AddAuthor(author);

        repository.DeleteAuthor(author.Id);

        Assert.Null(repository.GetAuthor(author.Id));
    }

    [Fact]
    public void PruneAuthors_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();

        Author orphan = new()
        {
            First = "David",
            Last = "Copperfield"
        };
        repository.AddAuthor(orphan);
        Author frank = new()
        {
            First = "Frank",
            Last = "Ross"
        };
        repository.AddAuthor(frank);
        // work with frank
        repository.AddWork(new Work
        {
            Authors = new List<WorkAuthor>(new[] { new WorkAuthor(frank) }),
            Key = "Frank 1963",
            Title = "Title",
            Language = "eng",
            YearPub = 1963
        });

        repository.PruneAuthors();

        Assert.Null(repository.GetAuthor(orphan.Id));
        Assert.NotNull(repository.GetAuthor(frank.Id));
    }

    private static IList<Author> GetSampleAuthors()
    {
        return new List<Author>(new[]
        {
            new Author
            {
                First = "John",
                Last = "Fairman"
            },
            new Author
            {
                First = "David",
                Last = "Suñas"
            },
            new Author
            {
                First = "Frank",
                Last = "Truman"
            },
        });
    }

    [Fact]
    public void GetAuthors_Unfiltered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Author author in GetSampleAuthors())
            repository.AddAuthor(author);

        var page = repository.GetAuthors(new AuthorFilter
        {
            PageNumber = 1,
            PageSize = 2
        });

        Assert.Equal(3, page.Total);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal("Fairman", page.Items[0].Last);
        Assert.Equal("Suñas", page.Items[1].Last);
    }

    [Fact]
    public void GetAuthors_Filtered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Author author in GetSampleAuthors())
            repository.AddAuthor(author);

        var page = repository.GetAuthors(new AuthorFilter
        {
            PageNumber = 1,
            PageSize = 2,
            Last = "man"
        });

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal("Fairman", page.Items[0].Last);
        Assert.Equal("Truman", page.Items[1].Last);
    }
    #endregion

    #region Containers
    private static Container GetSampleContainer()
    {
        return new Container
        {
            Key = null,
            Type = "journal",
            Title = "The Journal of Samples",
            Language = "eng",
            Edition = 1,
            Publisher = "Springer",
            YearPub = 2020,
            PlacePub = "Boston",
            Location = "www.jsa.org/123",
            AccessDate = new DateTime(2021, 1, 31).ToUniversalTime(),
            Number = "1",
            Note = "A note",
            Authors = new List<WorkAuthor>(new[]
            {
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe",
                    Suffix = "jr.",
                    Ordinal = 1,
                    Role = "editor"
                }
            }),
            Keywords = new List<Keyword>(new[]
            {
                new Keyword
                {
                    Language = "eng",
                    Value = "test"
                }
            })
        };
    }

    [Fact]
    public void AddContainer_NotExisting_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Container container = GetSampleContainer();

        repository.AddContainer(container);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, container.Id);
        Assert.NotNull(container.Key);

        var container2 = repository.GetContainer(container.Id);
        Assert.NotNull(container2);
        Assert.Equal(container.Key, container2.Key);
        Assert.Equal(container.Type, container2.Type);
        Assert.Equal(container.Title, container2.Title);
        Assert.Equal(container.Language, container2.Language);
        Assert.Equal(container.Edition, container2.Edition);
        Assert.Equal(container.Publisher, container2.Publisher);
        Assert.Equal(container.YearPub, container2.YearPub);
        Assert.Equal(container.PlacePub, container2.PlacePub);
        Assert.Equal(container.Location, container2.Location);
        Assert.Equal(container.AccessDate, container2.AccessDate);
        Assert.Equal(container.Number, container2.Number);
        Assert.Equal(container.Note, container2.Note);
        Assert.Equal(container.Authors.Count, container2.Authors.Count);
        Assert.Equal(container.Keywords.Count, container2.Keywords.Count);
    }

    [Fact]
    public void AddContainer_ExistingAuthor_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
            Suffix = "jr.",
        };
        repository.AddAuthor(author);
        Container container = GetSampleContainer();
        container.Authors[0].Id = author.Id;

        repository.AddContainer(container);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, container.Id);
        Assert.NotNull(container.Key);

        var container2 = repository.GetContainer(container.Id);
        Assert.NotNull(container2);
        Assert.Equal(container.Key, container2.Key);
        Assert.Equal(container.Type, container2.Type);
        Assert.Equal(container.Title, container2.Title);
        Assert.Equal(container.Language, container2.Language);
        Assert.Equal(container.Edition, container2.Edition);
        Assert.Equal(container.Publisher, container2.Publisher);
        Assert.Equal(container.YearPub, container2.YearPub);
        Assert.Equal(container.PlacePub, container2.PlacePub);
        Assert.Equal(container.Location, container2.Location);
        Assert.Equal(container.AccessDate, container2.AccessDate);
        Assert.Equal(container.Number, container2.Number);
        Assert.Equal(container.Note, container2.Note);
        Assert.Equal(container.Authors.Count, container2.Authors.Count);
        Assert.Equal(container.Keywords.Count, container2.Keywords.Count);
    }

    [Fact]
    public void AddContainer_ExistingAuthorUpdated_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
        };
        repository.AddAuthor(author);
        Container container = GetSampleContainer();
        // we are implicitly updating the author's suffix
        container.Authors[0].Id = author.Id;

        repository.AddContainer(container);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, container.Id);
        Assert.NotNull(container.Key);

        // author was updated
        Author author2 = repository.GetAuthor(author.Id);
        Assert.NotNull(author2);
        Assert.Equal(author.First, author2.First);
        Assert.Equal(author.Last, author2.Last);
        Assert.NotEqual(author.Suffix, author2.Suffix);

        var container2 = repository.GetContainer(container.Id);
        Assert.NotNull(container2);
        Assert.Equal(container.Key, container2.Key);
        Assert.Equal(container.Type, container2.Type);
        Assert.Equal(container.Title, container2.Title);
        Assert.Equal(container.Language, container2.Language);
        Assert.Equal(container.Edition, container2.Edition);
        Assert.Equal(container.Publisher, container2.Publisher);
        Assert.Equal(container.YearPub, container2.YearPub);
        Assert.Equal(container.PlacePub, container2.PlacePub);
        Assert.Equal(container.Location, container2.Location);
        Assert.Equal(container.AccessDate, container2.AccessDate);
        Assert.Equal(container.Number, container2.Number);
        Assert.Equal(container.Note, container2.Note);
        Assert.Equal(container.Authors.Count, container2.Authors.Count);
        Assert.Equal(container.Keywords.Count, container2.Keywords.Count);
    }

    [Fact]
    public void AddContainer_ExistingType_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        WorkType type = new()
        {
            Id = "journal",
            Name = "Journal"
        };
        repository.AddWorkType(type);
        Container container = GetSampleContainer();

        repository.AddContainer(container);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, container.Id);
        Assert.NotNull(container.Key);

        var container2 = repository.GetContainer(container.Id);
        Assert.NotNull(container2);
        Assert.Equal(container.Key, container2.Key);
        Assert.Equal(container.Type, container2.Type);
        Assert.Equal(container.Title, container2.Title);
        Assert.Equal(container.Language, container2.Language);
        Assert.Equal(container.Edition, container2.Edition);
        Assert.Equal(container.Publisher, container2.Publisher);
        Assert.Equal(container.YearPub, container2.YearPub);
        Assert.Equal(container.PlacePub, container2.PlacePub);
        Assert.Equal(container.Location, container2.Location);
        Assert.Equal(container.AccessDate, container2.AccessDate);
        Assert.Equal(container.Number, container2.Number);
        Assert.Equal(container.Note, container2.Note);
        Assert.Equal(container.Authors.Count, container2.Authors.Count);
        Assert.Equal(container.Keywords.Count, container2.Keywords.Count);
    }

    [Fact]
    public void AddContainer_Existing_Updated()
    {
        ResetDatabase();
        var repository = GetRepository();
        Container container = GetSampleContainer();
        repository.AddContainer(container);
        Guid id = container.Id;
        string key = container.Key;

        container.Title = "A new title";
        container.YearPub = 2021;
        repository.AddContainer(container);

        // ID is equal, key has changed
        Assert.Equal(id, container.Id);
        Assert.NotNull(container.Key);
        Assert.NotEqual(key, container.Key);

        var container2 = repository.GetContainer(container.Id);
        Assert.NotNull(container2);
        Assert.Equal(container.Key, container2.Key);
        Assert.Equal(container.Type, container2.Type);
        Assert.Equal(container.Title, container2.Title);
        Assert.Equal(container.Language, container2.Language);
        Assert.Equal(container.Edition, container2.Edition);
        Assert.Equal(container.Publisher, container2.Publisher);
        Assert.Equal(container.YearPub, container2.YearPub);
        Assert.Equal(container.PlacePub, container2.PlacePub);
        Assert.Equal(container.Location, container2.Location);
        Assert.Equal(container.AccessDate, container2.AccessDate);
        Assert.Equal(container.Number, container2.Number);
        Assert.Equal(container.Note, container2.Note);
        Assert.Equal(container.Authors.Count, container2.Authors.Count);
        Assert.Equal(container.Keywords.Count, container2.Keywords.Count);
    }

    [Fact]
    public void DeleteContainer_NotExisting_Nope()
    {
        ResetDatabase();
        var repository = GetRepository();
        Container container = GetSampleContainer();
        repository.AddContainer(container);

        repository.DeleteContainer(Guid.NewGuid());

        Assert.NotNull(repository.GetContainer(container.Id));
    }

    [Fact]
    public void DeleteContainer_Existing_Deleted()
    {
        ResetDatabase();
        var repository = GetRepository();
        Container container = GetSampleContainer();
        repository.AddContainer(container);

        repository.DeleteContainer(container.Id);

        Assert.Null(repository.GetContainer(container.Id));
    }

    private static IList<Container> GetSampleContainers()
    {
        return new List<Container>(new[]
        {
            new Container
            {
                Key = null,
                Type = "journal",
                Title = "The Journal of Samples",
                Language = "eng",
                YearPub = 2020,
                Number = "1",
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Suffix = "jr.",
                        Ordinal = 1,
                        Role = "editor"
                    }
                }),
                Keywords = new List<Keyword>(new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "test"
                    }
                })
            },
            new Container
            {
                Key = null,
                Type = "journal",
                Title = "The Pit",
                Language = "eng",
                YearPub = 2010,
                Number = "12",
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "Bob",
                        Last = "Charles",
                        Ordinal = 1,
                        Role = "editor"
                    }
                }),
                Keywords = new List<Keyword>(new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "test"
                    }
                })
            },
            new Container
            {
                Key = null,
                Type = "procs",
                Title = "Atti del convegno X",
                Language = "ita",
                YearPub = 2011,
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Suffix = "jr.",
                        Ordinal = 1,
                        Role = "editor"
                    }
                }),
                Keywords = new List<Keyword>(new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "another"
                    }
                })
            },
        });
    }

    [Fact]
    public void GetContainers_AndFiltered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Container container in GetSampleContainers())
            repository.AddContainer(container);

        var page = repository.GetContainers(new WorkFilter
        {
            IsMatchAnyEnabled = false,
            LastName = "Doe",
            Language = "eng"
        });

        Assert.Equal(1, page.Total);
        Assert.Equal(1, page.Items.Count);
        Assert.Equal("The Journal of Samples", page.Items[0].Title);
    }

    [Fact]
    public void GetContainers_OrFiltered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Container container in GetSampleContainers())
            repository.AddContainer(container);

        var page = repository.GetContainers(new WorkFilter
        {
            IsMatchAnyEnabled = true,
            LastName = "Doe",
            YearPubMax = 1900
        });

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal("Atti del convegno X", page.Items[0].Title);
        Assert.Equal("The Journal of Samples", page.Items[1].Title);
    }
    #endregion

    #region Works
    private static Work GetSampleWork()
    {
        return new Work
        {
            Key = null,
            Type = "book",
            Title = "A Beautiful Book",
            Language = "eng",
            Edition = 1,
            Publisher = "Springer",
            YearPub = 2020,
            PlacePub = "Boston",
            Location = "www.bestbooknest.com/123",
            Note = "A note",
            FirstPage = 1,
            LastPage = 123,
            Authors = new List<WorkAuthor>(new[]
            {
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe",
                    Suffix = "jr.",
                    Ordinal = 1,
                    Role = "editor"
                }
            }),
            Keywords = new List<Keyword>(new[]
            {
                new Keyword
                {
                    Language = "eng",
                    Value = "test"
                }
            })
        };
    }

    [Fact]
    public void AddWork_NotExisting_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Work work = GetSampleWork();

        repository.AddWork(work);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, work.Id);
        Assert.NotNull(work.Key);

        var work2 = repository.GetWork(work.Id);
        Assert.NotNull(work2);
        Assert.Equal(work.Key, work2.Key);
        Assert.Equal(work.Type, work2.Type);
        Assert.Equal(work.Title, work2.Title);
        Assert.Equal(work.Language, work2.Language);
        Assert.Equal(work.Edition, work2.Edition);
        Assert.Equal(work.Publisher, work2.Publisher);
        Assert.Equal(work.YearPub, work2.YearPub);
        Assert.Equal(work.PlacePub, work2.PlacePub);
        Assert.Equal(work.Location, work2.Location);
        Assert.Equal(work.AccessDate, work2.AccessDate);
        Assert.Equal(work.Note, work2.Note);
        Assert.Equal(work.FirstPage, work2.FirstPage);
        Assert.Equal(work.LastPage, work2.LastPage);
        Assert.Equal(work.Authors.Count, work2.Authors.Count);
        Assert.Equal(work.Keywords.Count, work2.Keywords.Count);
    }

    [Fact]
    public void AddWork_NotExistingWithContainer_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Work work = GetSampleWork();
        work.Container = GetSampleContainer();

        repository.AddWork(work);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, work.Id);
        Assert.NotNull(work.Key);

        var work2 = repository.GetWork(work.Id);
        Assert.NotNull(work2);
        Assert.NotNull(work2.Container);
        Assert.Equal(work.Key, work2.Key);
        Assert.Equal(work.Type, work2.Type);
        Assert.Equal(work.Title, work2.Title);
        Assert.Equal(work.Language, work2.Language);
        Assert.Equal(work.Edition, work2.Edition);
        Assert.Equal(work.Publisher, work2.Publisher);
        Assert.Equal(work.YearPub, work2.YearPub);
        Assert.Equal(work.PlacePub, work2.PlacePub);
        Assert.Equal(work.Location, work2.Location);
        Assert.Equal(work.AccessDate, work2.AccessDate);
        Assert.Equal(work.Note, work2.Note);
        Assert.Equal(work.FirstPage, work2.FirstPage);
        Assert.Equal(work.LastPage, work2.LastPage);
        Assert.Equal(work.Authors.Count, work2.Authors.Count);
        Assert.Equal(work.Keywords.Count, work2.Keywords.Count);
    }

    [Fact]
    public void AddWork_ExistingAuthor_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
            Suffix = "jr."
        };
        repository.AddAuthor(author);
        Work work = GetSampleWork();
        work.Authors[0].Id = author.Id;

        repository.AddWork(work);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, work.Id);
        Assert.NotNull(work.Key);

        var work2 = repository.GetWork(work.Id);
        Assert.NotNull(work2);
        Assert.Equal(work.Key, work2.Key);
        Assert.Equal(work.Type, work2.Type);
        Assert.Equal(work.Title, work2.Title);
        Assert.Equal(work.Language, work2.Language);
        Assert.Equal(work.Edition, work2.Edition);
        Assert.Equal(work.Publisher, work2.Publisher);
        Assert.Equal(work.YearPub, work2.YearPub);
        Assert.Equal(work.PlacePub, work2.PlacePub);
        Assert.Equal(work.Location, work2.Location);
        Assert.Equal(work.AccessDate, work2.AccessDate);
        Assert.Equal(work.FirstPage, work2.FirstPage);
        Assert.Equal(work.LastPage, work2.LastPage);
        Assert.Equal(work.Note, work2.Note);
        Assert.Equal(work.Authors.Count, work2.Authors.Count);
        Assert.Equal(work.Keywords.Count, work2.Keywords.Count);
    }

    [Fact]
    public void AddWork_ExistingAuthorUpdated_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        Author author = new()
        {
            First = "John",
            Last = "Doe",
        };
        repository.AddAuthor(author);
        Work work = GetSampleWork();
        // we are implicitly updating the author's suffix
        work.Authors[0].Id = author.Id;

        repository.AddWork(work);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, work.Id);
        Assert.NotNull(work.Key);

        // author was updated
        Author author2 = repository.GetAuthor(author.Id);
        Assert.NotNull(author2);
        Assert.Equal(author.First, author2.First);
        Assert.Equal(author.Last, author2.Last);
        Assert.NotEqual(author.Suffix, author2.Suffix);

        var work2 = repository.GetWork(work.Id);
        Assert.NotNull(work2);
        Assert.Equal(work.Key, work2.Key);
        Assert.Equal(work.Type, work2.Type);
        Assert.Equal(work.Title, work2.Title);
        Assert.Equal(work.Language, work2.Language);
        Assert.Equal(work.Edition, work2.Edition);
        Assert.Equal(work.Publisher, work2.Publisher);
        Assert.Equal(work.YearPub, work2.YearPub);
        Assert.Equal(work.PlacePub, work2.PlacePub);
        Assert.Equal(work.Location, work2.Location);
        Assert.Equal(work.AccessDate, work2.AccessDate);
        Assert.Equal(work.FirstPage, work2.FirstPage);
        Assert.Equal(work.LastPage, work2.LastPage);
        Assert.Equal(work.Note, work2.Note);
        Assert.Equal(work.Authors.Count, work2.Authors.Count);
        Assert.Equal(work.Keywords.Count, work2.Keywords.Count);
    }

    [Fact]
    public void AddWork_ExistingType_Added()
    {
        ResetDatabase();
        var repository = GetRepository();
        WorkType type = new()
        {
            Id = "journal",
            Name = "Journal"
        };
        repository.AddWorkType(type);
        Work work = GetSampleWork();

        repository.AddWork(work);

        // ID and key were updated
        Assert.NotEqual(Guid.Empty, work.Id);
        Assert.NotNull(work.Key);

        var work2 = repository.GetWork(work.Id);
        Assert.NotNull(work2);
        Assert.Equal(work.Key, work2.Key);
        Assert.Equal(work.Type, work2.Type);
        Assert.Equal(work.Title, work2.Title);
        Assert.Equal(work.Language, work2.Language);
        Assert.Equal(work.Edition, work2.Edition);
        Assert.Equal(work.Publisher, work2.Publisher);
        Assert.Equal(work.YearPub, work2.YearPub);
        Assert.Equal(work.PlacePub, work2.PlacePub);
        Assert.Equal(work.Location, work2.Location);
        Assert.Equal(work.AccessDate, work2.AccessDate);
        Assert.Equal(work.FirstPage, work2.FirstPage);
        Assert.Equal(work.LastPage, work2.LastPage);
        Assert.Equal(work.Note, work2.Note);
        Assert.Equal(work.Authors.Count, work2.Authors.Count);
        Assert.Equal(work.Keywords.Count, work2.Keywords.Count);
    }

    [Fact]
    public void AddWork_Existing_Updated()
    {
        ResetDatabase();
        var repository = GetRepository();
        Work work = GetSampleWork();
        repository.AddWork(work);
        Guid id = work.Id;
        string key = work.Key;

        work.Title = "A new title";
        work.YearPub = 2021;
        repository.AddWork(work);

        // ID is equal, key has changed
        Assert.Equal(id, work.Id);
        Assert.NotNull(work.Key);
        Assert.NotEqual(key, work.Key);

        var work2 = repository.GetWork(work.Id);
        Assert.NotNull(work2);
        Assert.Equal(work.Key, work2.Key);
        Assert.Equal(work.Type, work2.Type);
        Assert.Equal(work.Title, work2.Title);
        Assert.Equal(work.Language, work2.Language);
        Assert.Equal(work.Edition, work2.Edition);
        Assert.Equal(work.Publisher, work2.Publisher);
        Assert.Equal(work.YearPub, work2.YearPub);
        Assert.Equal(work.PlacePub, work2.PlacePub);
        Assert.Equal(work.Location, work2.Location);
        Assert.Equal(work.AccessDate, work2.AccessDate);
        Assert.Equal(work.FirstPage, work2.FirstPage);
        Assert.Equal(work.LastPage, work2.LastPage);
        Assert.Equal(work.Note, work2.Note);
        Assert.Equal(work.Authors.Count, work2.Authors.Count);
        Assert.Equal(work.Keywords.Count, work2.Keywords.Count);
    }

    [Fact]
    public void DeleteWork_NotExisting_Nope()
    {
        ResetDatabase();
        var repository = GetRepository();
        Work work = GetSampleWork();
        repository.AddWork(work);

        repository.DeleteWork(Guid.NewGuid());

        Assert.NotNull(repository.GetWork(work.Id));
    }

    [Fact]
    public void DeleteWork_Existing_Deleted()
    {
        ResetDatabase();
        var repository = GetRepository();
        Work work = GetSampleWork();
        repository.AddWork(work);

        repository.DeleteWork(work.Id);

        Assert.Null(repository.GetWork(work.Id));
    }

    private static IList<Work> GetSampleWorks()
    {
        return new List<Work>(new[]
        {
            new Work
            {
                Key = null,
                Type = "book",
                Title = "The Alpha",
                Language = "eng",
                YearPub = 2020,
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Suffix = "jr.",
                        Ordinal = 1,
                        Role = "editor"
                    }
                }),
                Keywords = new List<Keyword>(new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "test"
                    }
                })
            },
            new Work
            {
                Key = null,
                Type = "book",
                Title = "The Beta",
                Language = "eng",
                YearPub = 2010,
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "Bob",
                        Last = "Charles",
                        Ordinal = 1,
                        Role = "editor"
                    }
                }),
                Keywords = new List<Keyword>(new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "test"
                    }
                })
            },
            new Work
            {
                Key = null,
                Type = "paper",
                Title = "Il gamma",
                Language = "ita",
                YearPub = 2011,
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Suffix = "jr.",
                        Ordinal = 1,
                        Role = "editor"
                    }
                }),
                Keywords = new List<Keyword>(new[]
                {
                    new Keyword
                    {
                        Language = "eng",
                        Value = "another"
                    }
                })
            },
        });
    }

    [Fact]
    public void GetWorks_AndFiltered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Work work in GetSampleWorks())
            repository.AddWork(work);

        var page = repository.GetWorks(new WorkFilter
        {
            IsMatchAnyEnabled = false,
            LastName = "Doe",
            Language = "eng"
        });

        Assert.Equal(1, page.Total);
        Assert.Equal(1, page.Items.Count);
        Assert.Equal("The Alpha", page.Items[0].Title);
    }

    [Fact]
    public void GetWorks_OrFiltered_Ok()
    {
        ResetDatabase();
        var repository = GetRepository();
        foreach (Work work in GetSampleWorks())
            repository.AddWork(work);

        var page = repository.GetWorks(new WorkFilter
        {
            IsMatchAnyEnabled = true,
            LastName = "Doe",
            YearPubMax = 1900
        });

        Assert.Equal(2, page.Total);
        Assert.Equal(2, page.Items.Count);
        Assert.Equal("Il gamma", page.Items[0].Title);
        Assert.Equal("The Alpha", page.Items[1].Title);
    }

    [Fact]
    public void AddWork_SameKey_Suffixed()
    {
        ResetDatabase();
        var repository = GetRepository();

        Work a = new()
        {
            Type = "paper",
            Title = "Alpha",
            Language = "eng",
            YearPub = 2011,
            Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Ordinal = 1,
                        Role = "editor"
                    }
                })
        };
        Work b = new()
        {
            Type = "paper",
            Title = "Beta",
            Language = "eng",
            YearPub = 2011,
            Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Ordinal = 1,
                        Role = "editor"
                    }
                })
        };

        repository.AddWork(a);
        Assert.NotNull(a.Key);
        Assert.Equal("Doe 2011", a.Key);

        repository.AddWork(b);
        Assert.NotNull(b.Key);
        Assert.Equal("Doe 2011b", b.Key);
    }
    #endregion
}
