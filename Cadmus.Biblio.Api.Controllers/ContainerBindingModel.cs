using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers
{
    /// <summary>
    /// Container model.
    /// </summary>
    /// <seealso cref="WorkBaseBindingModel" />
    public sealed class ContainerBindingModel : WorkBaseBindingModel
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        [MaxLength(50)]
        public string Number { get; set; }
    }
}
