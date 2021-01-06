using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cadmus.Biblio.Api.Controllers
{
    /// <summary>
    /// Base class for work models.
    /// </summary>
    public abstract class WorkBaseBindingModel
    {
        /// <summary>
        /// Gets or sets the identifier (36-chars GUID).
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Gets or sets an optional arbitrarily defined key to identify
        /// this work (e.g. Rossi 1963).
        /// </summary>
        [MaxLength(300)]
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the authors and/or contributors.
        /// To reference an existing author just set the
        /// <see cref="WorkAuthorBindingModel.Id"/> property.
        /// </summary>
        public List<WorkAuthorBindingModel> Authors { get; set; }

        /// <summary>
        /// Gets or sets the work's type ID (e.g. book, journal, etc.).
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the work's title.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the work's language.
        /// </summary>
        [Required]
        [MaxLength(3)]
        [RegularExpression("^[a-z]{3}$")]
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the work's edition number (0 if not applicable).
        /// </summary>
        public short? Edition { get; set; }

        /// <summary>
        /// Gets or sets the publisher(s).
        /// </summary>
        [MaxLength(50)]
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the year of publication.
        /// </summary>
        public short? YearPub { get; set; }

        /// <summary>
        /// Gets or sets the place(s) of publication.
        /// </summary>
        [MaxLength(100)]
        public string PlacePub { get; set; }

        /// <summary>
        /// Gets or sets the location ID for this bibliographic item, e.g.
        /// a URL or a DOI.
        /// </summary>
        [MaxLength(500)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the last access date. Used for web resources.
        /// </summary>
        public DateTime? AccessDate { get; set; }

        /// <summary>
        /// Gets or sets some optional notes.
        /// </summary>
        [MaxLength(500)]
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the optional keywords linked to this work.
        /// </summary>
        public List<KeywordBindingModel> Keywords { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkBaseBindingModel"/>
        /// class.
        /// </summary>
        protected WorkBaseBindingModel()
        {
            Authors = new List<WorkAuthorBindingModel>();
            Keywords = new List<KeywordBindingModel>();
        }
    }
}
