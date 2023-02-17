using System;

namespace Cadmus.Biblio.Core;

/// <summary>
/// An <see cref="Author"/> related to a work.
/// </summary>
/// <seealso cref="Author" />
public class WorkAuthor : Author
{
    /// <summary>
    /// Gets or sets the role of this author in the work.
    /// </summary>
    public string? Role { get; set; }

    /// <summary>
    /// Gets or sets the ordinal number of the author in a list.
    /// This defines the order in which several authors appear.
    /// </summary>
    public short Ordinal { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkAuthor"/> class.
    /// </summary>
    public WorkAuthor()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkAuthor"/> class.
    /// </summary>
    /// <param name="author">The author whose properties should be copied
    /// into this new author.</param>
    /// <exception cref="ArgumentNullException">author</exception>
    public WorkAuthor(Author author)
    {
        if (author == null) throw new ArgumentNullException(nameof(author));

        Id = author.Id;
        First = author.First;
        Last = author.Last;
        Suffix = author.Suffix;
        Ordinal = 1;
    }
}
