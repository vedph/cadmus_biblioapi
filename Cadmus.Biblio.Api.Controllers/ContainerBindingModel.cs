using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Container binding model.
/// </summary>
public class ContainerBindingModel
{
    /// <summary>
    /// Gets or sets the identifier (36-chars GUID).
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// Gets or sets an optional arbitrarily defined key to identify
    /// this work (e.g. Rossi 1963).
    /// </summary>
    [MaxLength(300)]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the authors and/or contributors.
    /// To reference an existing author just set the
    /// <see cref="WorkAuthorBindingModel.Id"/> property.
    /// </summary>
    public List<WorkAuthorBindingModel> Authors { get; set; } = [];

    /// <summary>
    /// Gets or sets the work's type ID (e.g. book, journal, etc.).
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the work's title.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the work's language.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the work's edition number (0 if not applicable).
    /// </summary>
    public short? Edition { get; set; }

    /// <summary>
    /// Gets or sets the publisher(s).
    /// </summary>
    [MaxLength(50)]
    public string? Publisher { get; set; }

    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    [MaxLength(50)]
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the year of publication.
    /// </summary>
    public short? YearPub { get; set; }

    /// <summary>
    /// Gets or sets the optional second year for a publication period.
    /// </summary>
    public short? YearPub2 { get; set; }

    /// <summary>
    /// Gets or sets the place(s) of publication.
    /// </summary>
    [MaxLength(100)]
    public string? PlacePub { get; set; }

    /// <summary>
    /// Gets or sets the location ID for this bibliographic item, e.g.
    /// a URL or a DOI.
    /// </summary>
    [MaxLength(500)]
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the last access date. Used for web resources.
    /// </summary>
    public DateTime? AccessDate { get; set; }

    /// <summary>
    /// Gets or sets some optional notes.
    /// </summary>
    [MaxLength(500)]
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the datation.
    /// </summary>
    [MaxLength(1000)]
    public string? Datation { get; set; }

    /// <summary>
    /// Gets or sets the datation value.
    /// </summary>
    public double? DatationValue { get; set; }

    /// <summary>
    /// Gets or sets the optional keywords linked to this work.
    /// </summary>
    public List<KeywordBindingModel> Keywords { get; set; } = [];

    /// <summary>
    /// Gets or sets the external links.
    /// </summary>
    public List<LinkBindingModel> Links { get; set; } = [];

    /// <summary>
    /// Converts to container model.
    /// </summary>
    /// <returns>Container.</returns>
    public Container ToContainer()
    {
        return new Container
        {
            Id = Id ?? Guid.Empty,
            Key = Key,
            Authors = Authors?.Count > 0
                ? Authors.ConvertAll(m => m.ToWorkAuthor())
                : [],
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
                : [],
            Links = Links?.Count > 0
                ? Links.ConvertAll(m => m.ToExternalId())
                : []
        };
    }
}
