using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers
{
    public sealed class KeywordFilterBindingModel : PagingOptionsBindingModel
    {
        [MaxLength(3)]
        public string Language { get; set; }

        [MaxLength(50)]
        public string Value { get; set; }
    }
}
