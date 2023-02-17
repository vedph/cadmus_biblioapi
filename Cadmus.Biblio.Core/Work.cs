namespace Cadmus.Biblio.Core;

/// <summary>
/// A work.
/// </summary>
public class Work : Container
{
    /// <summary>
    /// Gets or sets the optional work's container.
    /// </summary>
    public Container? Container { get; set; }

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
}
