using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Core;

/// <summary>
/// A work container. The <see cref="Work"/> class extends this class.
/// </summary>
public class Container
{
    /// <summary>
    /// Gets or sets the identifier (36-chars GUID).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets an optional arbitrarily defined key to identify
    /// this work (e.g. Rossi 1963).
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the authors and/or contributors.
    /// </summary>
    public List<WorkAuthor> Authors { get; set; } = [];

    /// <summary>
    /// Gets or sets the work's type ID (e.g. book, journal, etc.).
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets or sets the work's title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the work's language.
    /// </summary>
    public string? Language { get; set; }

    /// <summary>
    /// Gets or sets the work's edition number (0 if not applicable).
    /// </summary>
    public short Edition { get; set; }

    /// <summary>
    /// Gets or sets the publisher(s).
    /// </summary>
    public string? Publisher { get; set; }

    /// <summary>
    /// Gets or sets the number. This can be either a container number
    /// when we enter one record for each issue of a periodic publication,
    /// or a work number, when the container (e.g. a journal) is treated
    /// as a single record, valid for all its issues, and the issue number
    /// is rather specified in the work. The choice is up to the user.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the year of publication.
    /// </summary>
    public short YearPub { get; set; }

    /// <summary>
    /// Gets or sets the second year in a range representing the publication
    /// period, the first year being <see cref="YearPub"/>.
    /// </summary>
    public short? YearPub2 { get; set; }

    /// <summary>
    /// Gets or sets the place(s) of publication.
    /// </summary>
    public string? PlacePub { get; set; }

    /// <summary>
    /// Gets or sets the location ID for this bibliographic item, e.g.
    /// a URL or a DOI.
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Gets or sets the last access date. Used for web resources.
    /// </summary>
    public DateTime? AccessDate { get; set; }

    /// <summary>
    /// Gets or sets some optional notes.
    /// </summary>
    public string? Note { get; set; }

    /// <summary>
    /// Gets or sets the optional datation, used for historical works and
    /// expressed in a human-readable form.
    /// </summary>
    public string? Datation { get; set; }

    /// <summary>
    /// Gets or sets the value calculated from <see cref="Datation"/> for
    /// use with sorting or filtering.
    /// </summary>
    public double? DatationValue { get; set; }

    /// <summary>
    /// Gets or sets the optional keywords linked to this work.
    /// </summary>
    public List<Keyword> Keywords { get; set; } = [];

    /// <summary>
    /// Gets or sets the links to external IDs.
    /// </summary>
    public List<ExternalId> Links { get; set; } = [];

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"#{Id} [{Type}] {Title}";
    }
}
