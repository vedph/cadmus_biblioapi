namespace Cadmus.Biblio.Ef;

/// <summary>
/// A keyword entity.
/// </summary>
public class EfKeyword
{
    /// <summary>
    /// Gets or sets the internal identifier. This does not surface
    /// to the bibliographic API, as a keyword identity is equal to
    /// its language and value. Yet, for performance and simplicity here
    /// an additional autonumber ID is used.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the language (ISO 639-3).
    /// </summary>
    public string Language { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets or sets the indexable form of <see cref="Value"/>.
    /// </summary>
    public string Valuex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EfKeyword"/> class.
    /// </summary>
    public EfKeyword()
    {
        Language = "";
        Value = "";
        Valuex = "";
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"#{Id} [{Language}] {Value}";
    }
}
