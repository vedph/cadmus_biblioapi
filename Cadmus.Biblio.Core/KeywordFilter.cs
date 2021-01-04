using Fusi.Tools.Data;

namespace Cadmus.Biblio.Core
{
    public sealed class KeywordFilter : PagingOptions
    {
        public string Language { get; set; }
        public string Value { get; set; }
    }
}
