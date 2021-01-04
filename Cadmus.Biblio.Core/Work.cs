using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// A work.
    /// </summary>
    public class Work
    {
        /// <summary>
        /// Gets or sets the identifier used internally for this work.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the authors.
        /// </summary>
        public List<Author> Authors { get; set; }

        /// <summary>
        /// Gets or sets the contributors.
        /// </summary>
        public List<Author> Contributors { get; set; }

        /// <summary>
        /// Gets or sets an optional arbitrarily defined key to identify
        /// this work (e.g. Rossi 1963).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the work's type name.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the work's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the work's language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the work's container.
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets the work's edition number (0 if not applicable).
        /// </summary>
        public short Edition { get; set; }

        /// <summary>
        /// Gets or sets the number (usually referred to the container).
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Gets or sets the publisher(s).
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the year of publication.
        /// </summary>
        public short YearPub { get; set; }

        /// <summary>
        /// Gets or sets the place(s) of publication.
        /// </summary>
        public string PlacePub { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the last access date. Used for web resources.
        /// </summary>
        public DateTime? AccessDate { get; set; }

        /// <summary>
        /// Gets or sets the first page number (0 if not applicable).
        /// </summary>
        public short FirstPage { get; set; }

        /// <summary>
        /// Gets or sets the last page number (0 if not applicable).
        /// </summary>
        public short LastPage { get; set; }

        /// <summary>
        /// Gets or sets some optional notes.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the optional keywords linked to this work.
        /// </summary>
        public List<Keyword> Keywords { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Work"/> class.
        /// </summary>
        public Work()
        {
            Authors = new List<Author>();
            Contributors = new List<Author>();
            Keywords = new List<Keyword>();
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"#{Id} [{Type}] {Title}";
        }
    }
}
