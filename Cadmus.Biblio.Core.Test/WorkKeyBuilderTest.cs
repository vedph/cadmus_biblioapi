using System.Collections.Generic;
using Xunit;

namespace Cadmus.Biblio.Core.Test
{
    public sealed class WorkKeyBuilderTest
    {
        [Fact]
        public void Build_SingleAuthor_Ok()
        {
            string key = WorkKeyBuilder.Build(new Work
            {
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe"
                    }
                }),
                YearPub = 2020
            });

            Assert.Equal("Doe 2020", key);
        }

        [Fact]
        public void Build_SingleAuthorWithSuffix_Ok()
        {
            string key = WorkKeyBuilder.Build(new Work
            {
                Authors = new List<WorkAuthor>(new[]
                {
                    new WorkAuthor
                    {
                        First = "John",
                        Last = "Doe",
                        Suffix = "jr."
                    }
                }),
                YearPub = 2020
            });

            Assert.Equal("Doe jr. 2020", key);
        }

        [Fact]
        public void Build_MultiAuthors_Ok()
        {
            string key = WorkKeyBuilder.Build(new Work
            {
                Authors = new List<WorkAuthor>(new[]
                {
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
                }),
                YearPub = 2020
            });

            Assert.Equal("Doe & Aspen 2020", key);
        }

        [Fact]
        public void Build_MultiAuthorsNoOrdinals_Ok()
        {
            string key = WorkKeyBuilder.Build(new Work
            {
                Authors = new List<WorkAuthor>(new[]
                {
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
                }),
                YearPub = 2020
            });

            Assert.Equal("Aspen & Doe 2020", key);
        }
    }
}
