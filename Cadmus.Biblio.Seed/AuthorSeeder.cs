﻿using Bogus;
using Cadmus.Biblio.Core;
using Cadmus.Biblio.Ef;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Biblio.Seed
{
    public sealed class AuthorSeeder
    {
        public void Seed(IBiblioRepository repository, int count)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            Faker faker = new Faker();
            HashSet<Tuple<string, string>> names = new HashSet<Tuple<string, string>>();
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
                    .RuleFor(a => a.Id, Guid.NewGuid().ToString())
                    .RuleFor(a => a.First, first)
                    .RuleFor(a => a.Last, last)
                    .RuleFor(a => a.Suffix, (string)null)
                    .Generate();
                repository.AddAuthor(author);
                repeat = 0;
            }
        }
    }
}