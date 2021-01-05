using System;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Entity linking an <see cref="EfAuthor"/> to an <see cref="EfContainer"/>.
    /// </summary>
    public class EfAuthorContainer
    {
        /// <summary>
        /// Gets or sets the author identifier.
        /// </summary>
        public Guid AuthorId { get; set; }

        /// <summary>
        /// Gets or sets the author.
        /// </summary>
        public EfAuthor Author { get; set; }

        /// <summary>
        /// Gets or sets the container identifier.
        /// </summary>
        public Guid ContainerId { get; set; }

        /// <summary>
        /// Gets or sets the work.
        /// </summary>
        public EfContainer Container{ get; set; }

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
            return $"{AuthorId}-{ContainerId}" +
                (string.IsNullOrEmpty(Role) ? $" [{Role}]" : "");
        }
    }
}
