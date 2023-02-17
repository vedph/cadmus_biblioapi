using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core;

/// <summary>
/// Authors filter.
/// </summary>
/// <seealso cref="PagingOptions" />
public class AuthorFilter : PagingOptions
{
    /// <summary>
    /// Gets or sets any part of the last name to be matched.
    /// </summary>
    public string? Last { get; set; }
}
