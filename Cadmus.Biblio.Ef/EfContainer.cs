﻿using System.Collections.Generic;

namespace Cadmus.Biblio.Ef;

/// <summary>
/// Entity for a work's container.
/// </summary>
/// <seealso cref="EfWorkBase" />
public sealed class EfContainer : EfWorkBase
{
    /// <summary>
    /// Gets or sets the author-container links.
    /// </summary>
    public List<EfAuthorContainer>? AuthorContainers { get; set; }

    /// <summary>
    /// Gets or sets the keyword-container links.
    /// </summary>
    public List<EfKeywordContainer>? KeywordContainers { get; set; }

    /// <summary>
    /// Gets or sets the links.
    /// </summary>
    public List<EfContainerLink>? Links { get; set; }
}
