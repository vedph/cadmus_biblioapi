using Bogus;
using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Seed;

public sealed class AuthorSeeder
{
    public void Seed(IBiblioRepository repository, int count)
    {
        ArgumentNullException.ThrowIfNull(repository);

        Faker faker = new();
        HashSet<Tuple<string, string>> names = [];
        int repeat = 0;

        for (int n = 1; n <= count; n++)
        {
            string first = faker.Name.FirstName();
            string last = faker.Name.LastName();
            var t = Tuple.Create(first, last);
            if (names.Contains(t))
            {
                if (++repeat > 100) break;
                continue;
            }

            Author author = new Faker<Author>()
                .RuleFor(a => a.Id, Guid.NewGuid())
                .RuleFor(a => a.First, first)
                .RuleFor(a => a.Last, last)
                .RuleFor(a => a.Suffix, (string?)null)
                .Generate();
            repository.AddAuthor(author);
            repeat = 0;
        }
    }
}
