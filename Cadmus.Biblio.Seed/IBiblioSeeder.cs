using Cadmus.Biblio.Core;

namespace Cadmus.Biblio.Seed;

public interface IBiblioSeeder
{
    void Seed(IBiblioRepository repository, int count);
}
