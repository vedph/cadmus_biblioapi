namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// A work's author.
    /// </summary>
    public class Author
    {
        /// <summary>
        /// First name.
        /// </summary>
        public string First { get; set; }

        /// <summary>
        /// Last name.
        /// </summary>
        public string Last { get; set; }

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
            return $"{Last}, {First} [{Role}]";
        }
    }
}
