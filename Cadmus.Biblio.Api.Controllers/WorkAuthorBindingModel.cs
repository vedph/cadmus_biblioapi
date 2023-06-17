using Cadmus.Biblio.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Work's author.
/// </summary>
public sealed class WorkAuthorBindingModel
{
    /// <summary>
    /// The ID.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// First name.
    /// </summary>
    [MaxLength(50)]
    public string? First { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    [MaxLength(50)]
    public string? Last { get; set; }

    /// <summary>
    /// An optional, arbitrary suffix which can be appended
    /// to the name to disambiguate two authors with the same name.
    /// </summary>
    [MaxLength(50)]
    public string? Suffix { get; set; }

    /// <summary>
    /// Gets or sets the optional role of this author in the context
    /// of his work.
    /// </summary>
    [MaxLength(50)]
    public string? Role { get; set; }

    [Range(0, 100)]
    public short Ordinal { get; set; }

    /// <summary>
    /// Converts this model into a work-author model.
    /// </summary>
    /// <returns>Model.</returns>
    public WorkAuthor ToWorkAuthor()
    {
        return new WorkAuthor
        {
            Id = Id ?? Guid.Empty,
            First = First ?? "",
            Last = Last ?? "",
            Suffix = Suffix,
            Role = Role,
            Ordinal = Ordinal
        };
    }
}
