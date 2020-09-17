using System;
using System.Linq;
using System.Text;

namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// Work's key builder.
    /// </summary>
    public static class WorkKeyBuilder
    {
        /// <summary>
        /// Builds the key for the specified work.
        /// </summary>
        /// <param name="work">The work.</param>
        /// <exception cref="ArgumentNullException">work</exception>
        public static string Build(Work work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));

            StringBuilder sb = new StringBuilder();

            if (work.Authors?.Count > 0)
            {
                sb.Append(string.Join("; ",
                    from a in work.Authors
                    orderby a.Last, a.First
                    select $"{a.Last}, {a.First}"));
            }

            sb.Append(' ').Append(work.YearPub);

            return sb.ToString();
        }
    }
}
