﻿namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Entity linking an <see cref="EfAuthor"/> to an <see cref="EfWork"/>.
    /// </summary>
    public class EfAuthorWork
    {
        /// <summary>
        /// Gets or sets the author identifier.
        /// </summary>
        public string AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public EfAuthor Author { get; set; }

        /// <summary>
        /// Gets or sets the work identifier.
        /// </summary>
        public string WorkId { get; set; }

        /// <summary>
        /// Gets or sets the work.
        /// </summary>
        public EfWork Work { get; set; }

        /// <summary>
        /// Gets or sets the optional role, used with authors linked to a
        /// specific work. This can represent the role of the author in the
        /// bibliographic record, e.g. "editor", "translator", "organization",
        /// etc.
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{AuthorId}-{WorkId}" +
                (string.IsNullOrEmpty(Role) ? $" [{Role}]" : "");
        }
    }
}
