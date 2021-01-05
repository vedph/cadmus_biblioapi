using Bogus;
using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using System;

namespace Cadmus.Biblio.Seed
{
    public sealed class ContainerSeeder : IBiblioSeeder
    {
        public void Seed(IBiblioRepository repository, int count)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            DataPage<Author> authorsPage = repository.GetAuthors(new AuthorFilter
            {
                PageNumber = 1,
                PageSize = 20
            });
            DataPage<Keyword> keywordsPage = repository.GetKeywords(new KeywordFilter
            {
                PageNumber = 1,
                PageSize = 20
            });

            Faker faker = new Faker();

            for (int n = 1; n <= count; n++)
            {
                Container container = new Faker<Container>()
                    .RuleFor(c => c.Id, Guid.NewGuid())
                    .RuleFor(c => c.Type, f => f.PickRandom(WorkTypeSeeder.TypeIds))
                    .RuleFor(c => c.Title, f => f.Random.Words(3))
                    .RuleFor(c => c.Language, "eng")
                    .RuleFor(c => c.Edition, f => f.Random.Short(0, 3))
                    .RuleFor(c => c.Publisher, f => f.Company.CompanyName())
                    .RuleFor(c => c.YearPub,
                        f => (short)f.Random.Number(1900, DateTime.Now.Year))
                    .RuleFor(c => c.PlacePub, f => f.Address.City())
                    .RuleFor(c => c.Location,
                        f => f.Random.Bool()? f.Internet.Url() : null)
                    .RuleFor(c => c.AccessDate,
                        f => f.Random.Bool(0.2f)
                        ? (DateTime?)f.Date.Recent() : null)
                    .RuleFor(c => c.Number, f => $"{f.Random.Number(0, 100)}")
                    .RuleFor(c => c.Note, f => f.Random.Bool(0.2f)
                        ? f.Lorem.Sentence() : null)
                    .Generate();
                container.Key = WorkKeyBuilder.Build(container);

                // authors
                container.Authors.Add(new WorkAuthor
                {
                    Id = faker.PickRandom(authorsPage.Items).Id,
                    Role = "editor"
                });

                // keywords
                for (int k = 1; k <= faker.Random.Number(1, 2); k++)
                    container.Keywords.Add(faker.PickRandom(keywordsPage.Items));

                repository.AddContainer(container);
            }
        }
    }
}
