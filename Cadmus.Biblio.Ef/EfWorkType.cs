namespace Cadmus.Biblio.Ef;

/// <summary>
/// An entity representing the work's type.
/// </summary>
public sealed class EfWorkType
{
    /// <summary>
    /// Gets or sets the identifier. This is an arbitrary string representing
    /// the type (e.g. <c>book</c>).
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the human-readable name for the type.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{Name} ({Id})";
    }
}
