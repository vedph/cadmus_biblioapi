using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core
{
    public sealed class KeywordFilter : PagingOptions
    {
        /// <summary>
        /// The language (ISO 639-3) to match.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Any portion of the keyword's value.
        /// </summary>
        public string Value { get; set; }
    }
}
