using Cadmus.Biblio.Core;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

public sealed class KeywordBindingModel
{
    [Required]
    [MaxLength(3)]
    public string? Language { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Value { get; set; }

    public Keyword ToKeyword()
    {
        return new Keyword
        {
            Language = Language,
            Value = Value
        };
    }
}
