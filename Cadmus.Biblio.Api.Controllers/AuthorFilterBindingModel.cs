using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers
{
    public sealed class AuthorFilterBindingModel : PagingOptionsBindingModel
    {
        [MaxLength(50)]
        public string Last { get; set; }
    }
}
