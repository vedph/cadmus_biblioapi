using Cadmus.Biblio.Core;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// EF container link.
/// </summary>
public class EfContainerLink : ExternalId
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the source.
    /// </summary>
    public EfContainer? Source { get; set; }
}
