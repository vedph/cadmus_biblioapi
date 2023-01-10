using System;

namespace Cadmus.Biblio.Ef;

public sealed class EfKeywordContainer
{
    /// <summary>
    /// Gets or sets the keyword's internal identifier.
    /// </summary>
    public int KeywordId { get; set; }

    /// <summary>
    /// Gets or sets the keyword.
    /// </summary>
    public EfKeyword Keyword { get; set; }

    /// <summary>
    /// Gets or sets the work identifier.
    /// </summary>
    public Guid ContainerId { get; set; }

    /// <summary>
    /// Gets or sets the work.
    /// </summary>
    public EfContainer Container { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{KeywordId}-{ContainerId}";
    }
}
