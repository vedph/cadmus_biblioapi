using Fusi.Tools.Data;
using System.Collections.Generic;

namespace Cadmus.Biblio.Core
{
    /// <summary>
    /// Bibliographic repository interface.
    /// </summary>
    public interface IBiblioRepository
    {
        /// <summary>
        /// Gets the work by its internal ID.
        /// </summary>
        /// <param name="id">The work's internal identifier.</param>
        /// <returns>Work or null if not found</returns>
        Work GetWork(int id);

        /// <summary>
        /// Gets the specified page of filtered works.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The works page.</returns>
        DataPage<Work> GetWorks(WorkFilter filter);

        /// <summary>
        /// Adds or updates the specified work.
        /// </summary>
        /// <param name="work">The work. If new, its internal ID is 0.</param>
        /// <returns>The work's internal ID.</returns>
        int AddWork(Work work);

        /// <summary>
        /// Deletes the work with the specified internal ID.
        /// </summary>
        /// <param name="id">The work's internal identifier.</param>
        void DeleteWork(int id);

        /// <summary>
        /// Gets the first <paramref name="count"/> type names including
        /// <paramref name="name"/> in their name, in alphabetical order.
        /// </summary>
        /// <param name="name">The part of the name to be matched. It can be
        /// null or empty when you want to match any type.</param>
        /// <param name="count">The maximum count of desired results, or
        /// 0 to get all the types.</param>
        /// <returns>Type names.</returns>
        IList<string> GetTypes(string name, int count);

        /// <summary>
        /// Adds the type with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The type's internal ID.</returns>
        int AddType(string name);

        /// <summary>
        /// Deletes the type with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        void DeleteType(string name);

        /// <summary>
        /// Gets the first <paramref name="count"/> authors including
        /// <paramref name="last"/> in their last name.
        /// </summary>
        /// <param name="last">The part of the last name to be matched.</param>
        /// <param name="count">The maximum count of desired results.</param>
        /// <returns>Authors.</returns>
        IList<Author> GetAuthors(string last, int count);

        /// <summary>
        /// Prunes the authors by removing all the authors without any work.
        /// This can be used to shrink the database and remove unused authors
        /// from it, as authors get added when adding works.
        /// </summary>
        void PruneAuthors();
    }
}
