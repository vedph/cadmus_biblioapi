namespace Cadmus.Biblio.Core;

/// <summary>
/// An external identifier for a work or container.
/// </summary>
public class ExternalId
{
    /// <summary>
    /// Gets or sets the source (work or container) identifier.
    /// </summary>
    public string SourceId { get; set; } = "";

    /// <summary>
    /// Gets or sets the scope.
    /// </summary>
    public string Scope { get; set; } = "";

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; } = "";

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{Scope}:{Value}";
    }
}
