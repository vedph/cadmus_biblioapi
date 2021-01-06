using Cadmus.Biblio.Core;
using Microsoft.Extensions.Logging;
using System;

namespace Cadmus.Biblio.Seed
{
    /// <summary>
    /// Bibliographic database data seeder.
    /// </summary>
    public sealed class BiblioSeeder
    {
        private readonly IBiblioRepository _repository;

        /// <summary>
        /// Gets or sets the optional logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BiblioSeeder"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">repository</exception>
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
            {
                Logger?.LogInformation("Seeding biblio types");
                new WorkTypeSeeder().Seed(_repository, 0);
            }

            if (entities == null || entities.IndexOf('K') > -1)
            {
                Logger?.LogInformation("Seeding biblio keywords");
                new KeywordSeeder().Seed(_repository, count);
            }

            if (entities == null || entities.IndexOf('A') > -1)
            {
                Logger?.LogInformation("Seeding biblio authors");
                new AuthorSeeder().Seed(_repository, count);
            }

            if (entities == null || entities.IndexOf('C') > -1)
            {
                Logger?.LogInformation("Seeding biblio containers");
                new ContainerSeeder().Seed(_repository, count);
            }

            if (entities == null || entities.IndexOf('W') > -1)
            {
                Logger?.LogInformation("Seeding biblio works");
                new WorkSeeder().Seed(_repository, count);
            }
        }
    }
}
