using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

public sealed class KeywordFilterBindingModel : PagingOptionsBindingModel
{
    /// <summary>
    /// The language to be matched.
    /// </summary>
    [MaxLength(50)]
    public string? Language { get; set; }

    /// <summary>
    /// Any portion of the keyword's value, eventually prefixed with
    /// the 3-letters keyword's language code plus colon (e.g. <c>eng:test</c>).
    /// </summary>
    [MaxLength(50)]
    public string? Value { get; set; }
}
