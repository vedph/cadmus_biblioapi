using Cadmus.Biblio.Core;
using System.Collections.Generic;
using System;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Work binding model. This is a container binding model plus an optional
/// container and page numbers.
/// </summary>
/// <seealso cref="ContainerBindingModel" />
public sealed class WorkBindingModel : ContainerBindingModel
{
    /// <summary>
    /// Gets or sets the optional container.
    /// To reference an existing author just set the
    /// <see cref="ContainerBindingModel.Id"/> property.
    /// </summary>
    public ContainerBindingModel? Container { get; set; }

    /// <summary>
    /// Gets or sets the first page number in the container (0 if not
    /// applicable).
    /// </summary>
    public short? FirstPage { get; set; }

    /// <summary>
    /// Gets or sets the last page number in the container (0 if not
    /// applicable).
    /// </summary>
    public short? LastPage { get; set; }

    public Work ToWork()
    {
        return new Work
        {
            Id = Id ?? Guid.Empty,
            Key = Key,
            Authors = Authors?.Count > 0
                ? Authors.ConvertAll(m => m.ToWorkAuthor())
                : new List<WorkAuthor>(),
            Type = Type,
            Title = Title,
            Language = Language,
            Edition = Edition ?? 0,
            Publisher = Publisher,
            Number = Number,
            YearPub = YearPub ?? 0,
            YearPub2 = YearPub2 == 0 ? null : YearPub2,
            PlacePub = PlacePub,
            Location = Location,
            AccessDate = AccessDate,
            Note = Note,
            Datation = Datation,
            DatationValue = DatationValue,
            Keywords = Keywords?.Count > 0
                ? Keywords.ConvertAll(m => m.ToKeyword())
                : new List<Keyword>(),
            Links = Links?.Count > 0
                ? Links.ConvertAll(m => m.ToExternalId())
                : new List<ExternalId>(),
            Container = Container?.ToContainer(),
            FirstPage = FirstPage ?? 0,
            LastPage = LastPage ?? 0,
        };
    }
}
