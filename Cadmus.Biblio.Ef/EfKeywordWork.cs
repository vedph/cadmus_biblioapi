namespace Cadmus.Biblio.Ef;

/// <summary>
/// Entity linking an <see cref="EfKeyword"/> to an <see cref="EfWork"/>.
/// </summary>
public sealed class EfKeywordWork
{
    /// <summary>
    /// Gets or sets the keyword's internal identifier.
    /// </summary>
    public int KeywordId { get; set; }

    /// <summary>
    /// Gets or sets the keyword.
    /// </summary>
    public EfKeyword? Keyword { get; set; }

    /// <summary>
    /// Gets or sets the work identifier.
    /// </summary>
    public string WorkId { get; set; }

    /// <summary>
    /// Gets or sets the work.
    /// </summary>
    public EfWork? Work { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfKeywordWork"/> class.
    /// </summary>
    public EfKeywordWork()
    {
        WorkId = "";
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{KeywordId}-{WorkId}";
    }
}
