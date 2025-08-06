using System.Collections.Generic;
using Xunit;

namespace Cadmus.Biblio.Core.Test;

public sealed class WorkKeyBuilderTest
{
    [Fact]
    public void Build_SingleAuthor_Ok()
    {
        string key = WorkKeyBuilder.Build(new Work
        {
            Authors = new List<WorkAuthor>(
            [
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe"
                }
            ]),
            YearPub = 2020
        }, false);

        Assert.Equal("Doe 2020", key);
    }

    [Fact]
    public void Build_SingleContainer_Ok()
    {
        string key = WorkKeyBuilder.Build(new Container
        {
            Authors = new List<WorkAuthor>(
            [
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe"
                }
            ]),
            Number = "n.s.11",
            YearPub = 2020
        }, true);

        Assert.Equal("Doe n.s.11 2020", key);
    }

    [Fact]
    public void Build_SingleAuthorWithSuffix_Ok()
    {
        string key = WorkKeyBuilder.Build(new Work
        {
            Authors = new List<WorkAuthor>(
            [
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe",
                    Suffix = "jr."
                }
            ]),
            YearPub = 2020
        }, true);

        Assert.Equal("Doe jr. 2020", key);
    }

    [Fact]
    public void Build_MultiAuthors_Ok()
    {
        string key = WorkKeyBuilder.Build(new Work
        {
            Authors = new List<WorkAuthor>(
            [
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe",
                    Ordinal = 1
                },
                new WorkAuthor
                {
                    First = "Mike",
                    Last = "Aspen",
                    Ordinal = 2
                }
            ]),
            YearPub = 2020
        }, true);

        Assert.Equal("Doe & Aspen 2020", key);
    }

    [Fact]
    public void Build_MoreThan3Authors_Ok()
    {
        string key = WorkKeyBuilder.Build(new Work
        {
            Authors = new List<WorkAuthor>(
            [
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe",
                    Ordinal = 1
                },
                new WorkAuthor
                {
                    First = "Mike",
                    Last = "Aspen",
                    Ordinal = 2
                },
                new WorkAuthor
                {
                    First = "Josh",
                    Last = "Williams",
                    Ordinal = 3
                },
                new WorkAuthor
                {
                    First = "Judy",
                    Last = "Perth",
                    Ordinal = 4
                },
                new WorkAuthor
                {
                    First = "Zac",
                    Last = "Neuman",
                    Ordinal = 5
                },
            ]),
            YearPub = 2020
        }, true);

        Assert.Equal("Doe & Aspen & Williams & al. 2020", key);
    }

    [Fact]
    public void Build_MultiAuthorsNoOrdinals_Ok()
    {
        string key = WorkKeyBuilder.Build(new Work
        {
            Authors = new List<WorkAuthor>(
            [
                new WorkAuthor
                {
                    First = "John",
                    Last = "Doe",
                },
                new WorkAuthor
                {
                    First = "Mike",
                    Last = "Aspen",
                }
            ]),
            YearPub = 2020
        }, true);

        Assert.Equal("Aspen & Doe 2020", key);
    }

    private static Work GetSampleWork()
    {
        return new Work
        {
            Authors = new List<WorkAuthor>([ new WorkAuthor
            {
                First = "John",
                Last = "Doe"
            }
            ]),
            YearPub = 2020
        };
    }

    [Fact]
    public void PickKey_NewManual_New()
    {
        Work work = GetSampleWork();
        work.Key = "!new";

        string key = WorkKeyBuilder.PickKey("old", work, false);

        Assert.Equal("!new", key);
    }

    [Fact]
    public void PickKey_NewNullVsManual_Old()
    {
        Work work = GetSampleWork();

        string key = WorkKeyBuilder.PickKey("!old", work, false);

        Assert.Equal("!old", key);
    }

    [Fact]
    public void PickKey_NewNullVsNonManual_New()
    {
        Work work = GetSampleWork();

        string key = WorkKeyBuilder.PickKey("old", work, false);

        Assert.Equal("Doe 2020", key);
    }
}
