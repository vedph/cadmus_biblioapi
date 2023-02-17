using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

public sealed class WorkTypeBindingModel
{
    [Required]
    [MaxLength(20)]
    public string? Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Name { get; set; }
}
