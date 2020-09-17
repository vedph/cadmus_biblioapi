using System;
using System.Collections.Generic;
using System.Text;

namespace Cadmus.Biblio.Ef
{
    public sealed class EfContributorWork
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
