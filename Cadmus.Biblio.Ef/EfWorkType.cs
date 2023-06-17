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
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the human-readable name for the type.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfWorkType"/> class.
    /// </summary>
    public EfWorkType()
    {
        Id = "";
        Name = "";
    }

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
