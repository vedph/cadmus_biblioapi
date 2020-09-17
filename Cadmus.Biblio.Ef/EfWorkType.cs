namespace Cadmus.Biblio.Ef
{
    public sealed class EfWorkType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
