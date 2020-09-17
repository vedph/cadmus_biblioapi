namespace Cadmus.Biblio.Ef
{
    public sealed class EfAuthorWork
    {
        public int AuthorId { get; set; }
        public EfAuthor Author { get; set; }
        public int WorkId { get; set; }
        public EfWork Work { get; set; }
        public string Role { get; set; }

        public override string ToString()
        {
            return $"{AuthorId}-{WorkId}";
        }
    }
}
