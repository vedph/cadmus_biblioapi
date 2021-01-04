namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// An <see cref="Author"/> related to a work.
    /// </summary>
    /// <seealso cref="Author" />
    public class WorkAuthor : Author
    {
        /// <summary>
        /// Gets or sets the role of this author in the work.
        /// </summary>
        public string Role { get; set; }
    }
}
