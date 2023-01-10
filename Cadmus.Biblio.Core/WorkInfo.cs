using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Core;

/// <summary>
/// Essential information about a work or container.
/// </summary>
public class WorkInfo
{
    /// <summary>
    /// Gets or sets a value indicating whether this is a work or a
    /// container.
    /// </summary>
    public bool IsContainer { get; set; }

    /// <summary>
    /// Gets or sets the identifier (36-chars GUID).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets an optional arbitrarily defined key to identify
    /// this work (e.g. Rossi 1963).
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Gets or sets the authors and/or contributors.
    /// </summary>
    public List<WorkAuthor> Authors { get; set; }

    /// <summary>
    /// Gets or sets the work's type ID (e.g. book, journal, etc.).
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the work's title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets or sets the work's language.
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the work's edition number (0 if not applicable).
    /// </summary>
    public short Edition { get; set; }

    /// <summary>
    /// Gets or sets the year of publication.
    /// </summary>
    public short YearPub { get; set; }

    /// <summary>
    /// Gets or sets the place(s) of publication.
    /// </summary>
    public string PlacePub { get; set; }

    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// Gets or sets the first page number.
    /// </summary>
    public short FirstPage { get; set; }

    /// <summary>
    /// Gets or sets the last page number.
    /// </summary>
    public short LastPage { get; set; }

    /// <summary>
    /// Gets or sets the optional keywords linked to this work.
    /// </summary>
    public List<Keyword> Keywords { get; set; }

    /// <summary>
    /// Gets or sets the container.
    /// </summary>
    public WorkInfo Container { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkInfo"/> class.
    /// </summary>
    public WorkInfo()
    {
        Authors = new List<WorkAuthor>();
        Keywords = new List<Keyword>();
    }
}
