using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// Filter for <see cref="Work"/>'s.
    /// </summary>
    /// <seealso cref="PagingOptions" />
    public class WorkFilter : PagingOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to match this filter
        /// it is enough to match any of the specified properties. The default
        /// value is false, meaning that all the specified properties must
        /// be matched.
        /// </summary>
        public bool IsMatchAnyEnabled { get; set; }

        /// <summary>
        /// Gets or sets the work's type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets any portion of the last name to be matched (filtered).
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the language to be matched.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets any portion of the title to be matched (filtered).
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets any portion of the container to be matched (filtered).
        /// </summary>
        public string Container { get; set; }

        /// <summary>
        /// Gets or sets any portion of the keyword to be matched.
        /// </summary>
        public string Keyword { get; set; }

        /// <summary>
        /// Gets or sets the minimum year of publication.
        /// </summary>
        public short YearPubMin { get; set; }

        /// <summary>
        /// Gets or sets the maximum year of publication.
        /// </summary>
        public short YearPubMax { get; set; }

        /// <summary>
        /// Gets or sets the citation key to be matched.
        /// </summary>
        public string Key { get; set; }
    }
}
