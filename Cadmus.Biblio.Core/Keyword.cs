namespace Cadmus.Biblio.Core;

/// <summary>
/// A work's keyword.
/// </summary>
public class Keyword
{
    /// <summary>
    /// Gets or sets the language (ISO 639-3).
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the keyword's value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"[{Language}] {Value}";
    }
}
