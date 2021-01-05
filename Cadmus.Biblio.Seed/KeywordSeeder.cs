using Bogus;
using Cadmus.Biblio.Core;
using System;

namespace Cadmus.Biblio.Seed
{
    public sealed class KeywordSeeder : IBiblioSeeder
    {
        public void Seed(IBiblioRepository repository, int count)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));

            foreach (string word in new Faker().Random.WordsArray(count))
            {
                repository.AddKeyword(new Keyword
                {
                    Language = "eng",
                    Value = word
                });
            }
        }
    }
}
