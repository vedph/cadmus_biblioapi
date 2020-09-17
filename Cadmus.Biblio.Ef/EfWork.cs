using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Work.
    /// </summary>
    public sealed class EfWork
    {
        public int Id { get; set; }
        public int? TypeId { get; set; }
        public string Title { get; set; }
        public string Titlex { get; set; }
        public string Language { get; set; }
        public string Container { get; set; }
        public string Containerx { get; set; }
        public short Edition { get; set; }
        public string Number { get; set; }
        public string Publisher { get; set; }
        public short YearPub { get; set; }
        public string PlacePub { get; set; }
        public string Location { get; set; }
        public DateTime? AccessDate { get; set; }
        public short FirstPage { get; set; }
        public short LastPage { get; set; }
        public string Key { get; set; }
        public string Note { get; set; }

        public EfWorkType Type { get; set; }

        public List<EfKeyword> Keywords { get; set; }

        /// <summary>
        /// Gets or sets the author-works link.
        /// </summary>
        public List<EfAuthorWork> AuthorWorks { get; set; }

        /// <summary>
        /// Gets or sets the contributor-works link.
        /// </summary>
        public List<EfContributorWork> ContributorWorks { get; set; }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return $"[{TypeId}] {Title}";
        }
    }
}
