using System.Collections.Generic;

namespace Cadmus.Biblio.Ef
{
    public sealed class EfAuthor
    {
        /// <summary>
        /// ID (autonumber).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string Last { get; set; }

        /// <summary>
        /// Gets or sets the value of <see cref="Last"/> filtered for indexing.
        /// </summary>
        public string Lastx { get; set; }

        /// <summary>
        /// Gets or sets the author-works link.
        /// </summary>
        public List<EfAuthorWork> AuthorWorks { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"{Last}, {First} ({Id})";
        }
    }
}
