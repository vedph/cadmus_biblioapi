using Cadmus.Biblio.Core;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

public class LinkBindingModel
{
    [Required]
    [MaxLength(50)]
    public string? Scope { get; set; }

    [Required]
    [MaxLength(1000)]
    public string? Value { get; set; }

    public ExternalId ToExternalId()
    {
        return new ExternalId
        {
            Scope = Scope!,
            Value = Value!
        };
    }
}
