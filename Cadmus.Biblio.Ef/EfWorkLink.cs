using Cadmus.Biblio.Core;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// EF work link.
/// </summary>
public class EfWorkLink : ExternalId
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the source work.
    /// </summary>
    public EfWork? Source { get; set; }
}
