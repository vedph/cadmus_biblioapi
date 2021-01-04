using System;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Base class for <see cref="EfWork"/> and <see cref="EfContainer"/>.
    /// </summary>
    public class EfWorkBase
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets an optional arbitrarily defined key to identify
        /// this work (e.g. Rossi 1963).
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the work's type ID (e.g. book, journal, etc.).
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public EfWorkType Type { get; set; }

        /// <summary>
        /// Gets or sets the work's title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the filtered work's title.
        /// </summary>
        public string Titlex { get; set; }

        /// <summary>
        /// Gets or sets the work's language.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the work's edition number (0 if not applicable).
        /// </summary>
        public short Edition { get; set; }

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
        /// Gets or sets the location ID for this bibliographic item, e.g.
        /// a URL or a DOI.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the last access date. Used for web resources.
        /// </summary>
        public DateTime? AccessDate { get; set; }

        /// <summary>
        /// Gets or sets some optional notes.
        /// </summary>
        public string Note { get; set; }

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
