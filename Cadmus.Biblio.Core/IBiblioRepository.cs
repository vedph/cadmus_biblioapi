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
        /// Gets the specified page of filtered works.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The works page.</returns>
        DataPage<WorkInfo> GetWorks(WorkFilter filter);

        /// <summary>
        /// Gets the work by its ID.
        /// </summary>
        /// <param name="id">The work's identifier.</param>
        /// <returns>Work, or null if not found</returns>
        Work GetWork(string id);

        /// <summary>
        /// Adds or updates the specified work.
        /// Work type, authors, and keywords are stored too.
        /// </summary>
        /// <param name="work">The work.</param>
        void AddWork(Work work);

        /// <summary>
        /// Deletes the work with the specified ID.
        /// </summary>
        /// <param name="id">The work's identifier.</param>
        void DeleteWork(string id);

        /// <summary>
        /// Gets the specified page of filtered containers.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The containers page.</returns>
        DataPage<WorkInfo> GetContainers(WorkFilter filter);

        /// <summary>
        /// Gets the container by its ID.
        /// </summary>
        /// <param name="id">The container's identifier.</param>
        /// <returns>Container, or null if not found</returns>
        Container GetContainer(string id);

        /// <summary>
        /// Adds or updates the specified container.
        /// Container type, authors, and keywords are stored too.
        /// </summary>
        /// <param name="container">The container.</param>
        void AddContainer(Container container);

        /// <summary>
        /// Deletes the container with the specified ID.
        /// </summary>
        /// <param name="id">The container's identifier.</param>
        void DeleteContainer(string id);

        /// <summary>
        /// Gets the page of types matching the specified filter,
        /// or all of them when page size is 0.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The types page.</returns>
        DataPage<WorkType> GetTypes(WorkTypeFilter filter);

        /// <summary>
        /// Gets the type with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The type, or null if not found.</returns>
        WorkType GetType(string id);

        /// <summary>
        /// Adds or updates the type with the specified ID and name.
        /// </summary>
        /// <param name="type">The type.</param>
        void AddType(WorkType type);

        /// <summary>
        /// Deletes the type with the specified ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        void DeleteType(string id);

        /// <summary>
        /// Gets the specified page of authors matching <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>Page of authors.</returns>
        DataPage<Author> GetAuthors(AuthorFilter filter);

        /// <summary>
        /// Gets the author with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>The author, or null if not found.</returns>
        Author GetAuthor(string id);

        /// <summary>
        /// Adds or updates the specified author.
        /// </summary>
        /// <param name="author">The author.</param>
        void AddAuthor(Author author);

        /// <summary>
        /// Deletes the author with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void DeleteAuthor(string id);

        /// <summary>
        /// Prunes the authors by removing all the authors not assigned to
        /// any work or container.
        /// </summary>
        void PruneAuthors();

        /// <summary>
        /// Gets the specified page of keywords.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The page.</returns>
        DataPage<Keyword> GetKeywords(KeywordFilter filter);

        /// <summary>
        /// Prunes the keywords by removing all the keywords not assigned to
        /// any work.
        /// </summary>
        void PruneKeywords();
    }
}
