using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Container model.
/// </summary>
/// <seealso cref="WorkBaseBindingModel" />
public sealed class ContainerBindingModel : WorkBaseBindingModel
{
    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    [MaxLength(50)]
    public string? Number { get; set; }

    public Container ToContainer()
    {
        return new Container
        {
            Id = Id ?? Guid.Empty,
            Key = Key,
            Authors = Authors?.Count > 0
                ? Authors.ConvertAll(m => ModelHelper.GetAuthor(m)!)
                : new List<WorkAuthor>(),
            Type = Type,
            Title = Title,
            Language = Language,
            Edition = Edition ?? 0,
            Publisher = Publisher,
            YearPub = YearPub ?? 0,
            YearPub2 = YearPub2 == 0? null : YearPub2,
            PlacePub = PlacePub,
            Location = Location,
            AccessDate = AccessDate,
            Note = Note,
            Keywords = Keywords?.Count > 0
                ? Keywords.ConvertAll(m => ModelHelper.GetKeyword(m)!)
                : new List<Keyword>(),
            Number = Number
        };
    }
}
