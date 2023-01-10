using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// Work entity.
/// </summary>
public sealed class EfWork : EfWorkBase
{
    /// <summary>
    /// Gets or sets the author-work links.
    /// </summary>
    public List<EfAuthorWork> AuthorWorks { get; set; }

    /// <summary>
    /// Gets or sets the optional container identifier.
    /// </summary>
    public Guid? ContainerId { get; set; }

    /// <summary>
    /// Gets or sets the optional container.
    /// </summary>
    public EfContainer Container { get; set; }

    /// <summary>
    /// Gets or sets the first page number in the container (0 if not
    /// applicable).
    /// </summary>
    public short FirstPage { get; set; }

    /// <summary>
    /// Gets or sets the last page number in the container (0 if not
    /// applicable).
    /// </summary>
    public short LastPage { get; set; }

    /// <summary>
    /// Gets or sets the keyword-work links.
    /// </summary>
    public List<EfKeywordWork> KeywordWorks { get; set; }
}
