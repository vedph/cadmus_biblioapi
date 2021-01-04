using System;

namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// A container for a <see cref="Work"/>.
    /// </summary>
    public class Container : WorkBase
    {
        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Work"/> class.
        /// </summary>
        public Container()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Work"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <exception cref="ArgumentNullException">id</exception>
        public Container(string id) : base(id)
        {
        }
    }
}
