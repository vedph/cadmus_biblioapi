using System;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers
{
    public sealed class AuthorBindingModel
    {
        /// <summary>
        /// The ID.
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string First { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Last { get; set; }

        /// <summary>
        /// An optional, arbitrary suffix which can be appended
        /// to the name to disambiguate two authors with the same name.
        /// </summary>
        [MaxLength(50)]
        public string Suffix { get; set; }
    }
}
