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
        /// The manual-key prefix. When a work or container key starts with
        /// this prefix, it is not automatically updated by the system.
        /// </summary>
        public const string MAN_KEY_PREFIX = "!";

        /// <summary>
        /// Builds the key for the specified work.
        /// </summary>
        /// <param name="work">The work.</param>
        /// <exception cref="ArgumentNullException">work</exception>
        public static string Build(Container work)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));

            StringBuilder sb = new();

            // authors (max 3)
            if (work.Authors?.Count > 0)
            {
                sb.Append(string.Join(" & ",
                    (from a in work.Authors
                    orderby a.Ordinal, a.Last, a.Suffix
                    select string.IsNullOrEmpty(a.Suffix)
                        ? a.Last
                        : $"{a.Last} {a.Suffix}").Take(3)));

                if (work.Authors.Count > 3) sb.Append(" & al.");
            }

            // number if any
            if (!string.IsNullOrEmpty(work?.Number))
                sb.Append(' ').Append(work.Number);

            // year
            sb.Append(' ').Append(work.YearPub);

            // ensure we stay inside size limits
            return sb.Length > 300? sb.ToString(0, 300) : sb.ToString();
        }

        /// <summary>
        /// Picks the key by choosing between <paramref name="work"/>'s key
        /// and a new incoming key. A new manual-key always wins. Else, the
        /// new key is calculated, and then it wins if the old key is not
        /// manual; else the old key is picked.
        /// </summary>
        /// <param name="oldKey">The old key.</param>
        /// <param name="newWork">The new work/container; its key can be specified,
        /// or just be null.</param>
        /// <returns>Key.</returns>
        /// <exception cref="ArgumentNullException">work</exception>
        public static string PickKey(string oldKey, Container newWork)
        {
            if (newWork == null) throw new ArgumentNullException(nameof(newWork));

            // a new key with a manually-set value always wins
            if (newWork.Key?.StartsWith(MAN_KEY_PREFIX) == true)
                return newWork.Key;

            // else calculate the new key
            string newKey = Build(newWork);

            // if the existing key is not specified/is not manual, the new key wins;
            // else keep the existing key
            return oldKey?.StartsWith(MAN_KEY_PREFIX) != true ? newKey : oldKey;
        }
    }
}
