using System;
using System.Text;

namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// A work's author.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Gets or sets the identifier (36-chars GUID).
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// First name.
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string Last { get; set; }

        /// <summary>
        /// Gets or sets an optional, arbitrary suffix which can be appended
        /// to the name to disambiguate two authors with the same name.
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            StringBuilder sb = new();

            if (!string.IsNullOrEmpty(Last)) sb.Append(Last);
            if (!string.IsNullOrEmpty(Suffix))
                sb.Append(" (").Append(Suffix).Append(')');
            if (!string.IsNullOrEmpty(First)) sb.Append(", ").Append(First);

            return sb.ToString();
        }
    }
}
