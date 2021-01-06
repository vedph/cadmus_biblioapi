using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core
{
    public sealed class KeywordFilter : PagingOptions
    {
        /// <summary>
        /// The language (ISO 639-3).
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }
    }
}
