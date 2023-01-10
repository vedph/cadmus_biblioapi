using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core;

/// <summary>
/// A filter for work types.
/// </summary>
/// <seealso cref="PagingOptions" />
public class WorkTypeFilter : PagingOptions
{
    public string Name { get; set; }
}
