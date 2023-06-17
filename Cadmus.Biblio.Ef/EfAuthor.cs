using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// Author entity.
/// </summary>
/// <seealso cref="Author" />
public sealed class EfAuthor
{
    /// <summary>
    /// Gets or sets the identifier (36-chars GUID).
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// First name.
    /// </summary>
    public string First { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    public string Last { get; set; }

    /// <summary>
    /// Last name.
    /// </summary>
    public string Lastx { get; set; }

    /// <summary>
    /// Gets or sets an optional, arbitrary suffix which can be appended
    /// to the name to disambiguate two authors with the same name.
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfAuthor"/> class.
    /// </summary>
    public EfAuthor()
    {
        Id = Guid.NewGuid().ToString();
        First = "";
        Last = "";
        Lastx = "";
        AuthorWorks = new List<EfAuthorWork>();
    }

    /// <summary>
    /// Gets or sets the author-works link.
    /// </summary>
    public List<EfAuthorWork> AuthorWorks { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append(Last);
        if (!string.IsNullOrEmpty(Suffix))
            sb.Append(" (").Append(Suffix).Append(')');
        sb.Append(", ").Append(First);

        return sb.ToString();
    }
}
