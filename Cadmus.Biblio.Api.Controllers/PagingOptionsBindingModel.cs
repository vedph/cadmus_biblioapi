using Fusi.Tools.Data;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Paging options.
/// </summary>
public class PagingOptionsBindingModel : IPagingOptions
{
    /// <summary>
    /// The page number (1-N).
    /// </summary>
    [Range(1, int.MaxValue)]
    public int PageNumber { get; set; }

    /// <summary>
    /// The page size (0-100).
    /// </summary>
    [Range(0, 100)]
    public int PageSize { get; set; }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"{PageNumber}x{PageSize}";
    }
}
