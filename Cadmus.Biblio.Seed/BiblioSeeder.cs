using Cadmus.Biblio.Core;
using System;

namespace Cadmus.Biblio.Seed
{
    public sealed class BiblioSeeder
    {
        private readonly IBiblioRepository _repository;

        public BiblioSeeder(IBiblioRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public void Seed(int count)
        {
            new WorkTypeSeeder().Seed(_repository, 0);
            new KeywordSeeder().Seed(_repository, count);
            new AuthorSeeder().Seed(_repository, count);
            new ContainerSeeder().Seed(_repository, count);
            new WorkSeeder().Seed(_repository, count);
        }
    }
}
