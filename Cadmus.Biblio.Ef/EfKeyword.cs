using System;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// A keyword.
    /// </summary>
    public class EfKeyword
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the parent work's identifier.
        /// </summary>
        public int WorkId { get; set; }

        /// <summary>
        /// Gets or sets the language (ISO 639-3).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the indexable form of <see cref="Value"/>.
        /// </summary>
        public string Valuex { get; set; }

        /// <summary>
        /// Gets or sets the parent work.
        /// </summary>
        public EfWork Work { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"#{Id} [{Language}] {Value}";
        }
    }
}
