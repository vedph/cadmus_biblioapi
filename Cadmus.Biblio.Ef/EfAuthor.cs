using Cadmus.Biblio.Core;
using System;
using System.Collections.Generic;

namespace Cadmus.Biblio.Ef
{
    /// <summary>
    /// Author entity.
    /// </summary>
    /// <seealso cref="Author" />
    public sealed class EfAuthor : Author
    {
        /// <summary>
        /// Gets or sets the value of <see cref="Last"/> filtered for indexing.
        /// </summary>
        public string Lastx { get; set; }

        /// <summary>
        /// Gets or sets the author-works link.
        /// </summary>
        public List<EfAuthorWork> AuthorWorks { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfAuthor"/> class.
        /// </summary>
        public EfAuthor()
        {
            Id = Guid.NewGuid();
        }
    }
}
