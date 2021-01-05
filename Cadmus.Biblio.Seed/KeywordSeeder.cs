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
                int i = word.IndexOf(' ');
                string value = i > -1? word.Substring(0, i) : word;
                repository.AddKeyword(new Keyword
                {
                    Language = "eng",
                    Value = value
                });
            }
        }
    }
}
