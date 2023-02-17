using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core;

public sealed class KeywordFilter : PagingOptions
{
    /// <summary>
    /// The language (ISO 639-3) to match.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Any portion of the keyword's value, eventually prefixed with
    /// the 3-letters keyword's language code plus colon (e.g. <c>eng:test</c>).
    /// </summary>
    public string? Value { get; set; }
}
