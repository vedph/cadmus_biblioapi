namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Work.
/// </summary>
/// <seealso cref="WorkBaseBindingModel" />
public sealed class WorkBindingModel : WorkBaseBindingModel
{
    /// <summary>
    /// Gets or sets the optional container.
    /// To reference an existing author just set the
    /// <see cref="ContainerBindingModel.Id"/> property.
    /// </summary>
    public ContainerBindingModel Container { get; set; }

    /// <summary>
    /// Gets or sets the first page number in the container (0 if not
    /// applicable).
    /// </summary>
    public short? FirstPage { get; set; }

    /// <summary>
    /// Gets or sets the last page number in the container (0 if not
    /// applicable).
    /// </summary>
    public short? LastPage { get; set; }

}
