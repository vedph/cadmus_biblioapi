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

        /// <summary>
        /// Seeds the database with the specified count of entries for each
        /// type.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="entities">The optional selector for the entities to
        /// be seeded; in this string T=types, K=keywords, A=authors,
        /// C=containers, W=works. When specified, only the entities listed
        /// in this string are seeded.</param>
        public void Seed(int count, string entities = null)
        {
            if (entities == null || entities.IndexOf('T') > -1)
                new WorkTypeSeeder().Seed(_repository, 0);

            if (entities == null || entities.IndexOf('K') > -1)
                new KeywordSeeder().Seed(_repository, count);

            if (entities == null || entities.IndexOf('A') > -1)
                new AuthorSeeder().Seed(_repository, count);

            if (entities == null || entities.IndexOf('C') > -1)
                new ContainerSeeder().Seed(_repository, count);

            if (entities == null || entities.IndexOf('W') > -1)
                new WorkSeeder().Seed(_repository, count);
        }
    }
}
