namespace Cadmus.Biblio.Ef
{
    public sealed class EfKeywordWork
    {
        public int KeywordId { get; set; }
        public EfKeyword Keyword { get; set; }
        public int WorkId { get; set; }
        public EfWork Work { get; set; }

        public override string ToString()
        {
            return $"{KeywordId}-{WorkId}";
        }
    }
}
