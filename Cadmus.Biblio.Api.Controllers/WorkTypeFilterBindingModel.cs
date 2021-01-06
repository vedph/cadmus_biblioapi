using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers
{
    public sealed class WorkTypeFilterBindingModel : PagingOptionsBindingModel
    {
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
