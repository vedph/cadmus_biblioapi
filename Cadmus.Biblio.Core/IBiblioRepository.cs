﻿using Fusi.Tools.Data;
using System;

namespace Cadmus.Biblio.Core;

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
    Work? GetWork(Guid id);

    /// <summary>
    /// Adds or updates the specified work.
    /// Work type, container, authors, and keywords are stored too.
    /// As for authors, you can specify only the author's ID to assign
    /// the work to an existing author; otherwise, the author will be
    /// either added or updated as required. Keywords are added or
    /// updated as required. Type is added if not found, even this should
    /// not happen, as types are a predefined set. As for the container,
    /// you can specify only its ID to assign the work to it; otherwise,
    /// the container will be added or updated as required.
    /// </summary>
    /// <param name="work">The work.</param>
    void AddWork(Work work);

    /// <summary>
    /// Deletes the work with the specified ID.
    /// </summary>
    /// <param name="id">The work's identifier.</param>
    void DeleteWork(Guid id);

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
    Container? GetContainer(Guid id);

    /// <summary>
    /// Adds or updates the specified container.
    /// Container type, authors, and keywords are stored too.
    /// As for authors, you can specify only the author's ID to assign
    /// the work to an existing author; otherwise, the author will be
    /// either added or updated as required. Keywords are added or
    /// updated as required. Type is added if not found, even this should
    /// not happen, as types are a predefined set.
    /// </summary>
    /// <param name="container">The container.</param>
    void AddContainer(Container container);

    /// <summary>
    /// Deletes the container with the specified ID.
    /// </summary>
    /// <param name="id">The container's identifier.</param>
    void DeleteContainer(Guid id);

    /// <summary>
    /// Gets the page of types matching the specified filter,
    /// or all of them when page size is 0.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The types page.</returns>
    DataPage<WorkType> GetWorkTypes(WorkTypeFilter filter);

    /// <summary>
    /// Gets the type with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The type, or null if not found.</returns>
    WorkType? GetWorkType(string id);

    /// <summary>
    /// Adds or updates the type with the specified ID and name.
    /// </summary>
    /// <param name="type">The type.</param>
    void AddWorkType(WorkType type);

    /// <summary>
    /// Deletes the type with the specified ID.
    /// </summary>
    /// <param name="id">The ID.</param>
    void DeleteWorkType(string id);

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
    Author? GetAuthor(Guid id);

    /// <summary>
    /// Adds or updates the specified author.
    /// </summary>
    /// <param name="author">The author.</param>
    void AddAuthor(Author author);

    /// <summary>
    /// Deletes the author with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void DeleteAuthor(Guid id);

    /// <summary>
    /// Prunes the authors by removing all the authors not assigned to
    /// any work or container.
    /// </summary>
    void PruneAuthors();

    /// <summary>
    /// Gets the keyword with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>The keyword or null if not found.</returns>
    Keyword? GetKeyword(int id);

    /// <summary>
    /// Gets the specified page of keywords.
    /// </summary>
    /// <param name="filter">The filter.</param>
    /// <returns>The page.</returns>
    DataPage<Keyword> GetKeywords(KeywordFilter filter);

    /// <summary>
    /// Adds or updates the specified keyword.
    /// </summary>
    /// <param name="keyword">The keyword.</param>
    /// <returns>The keyword's ID.</returns>
    int AddKeyword(Keyword keyword);

    /// <summary>
    /// Deletes the keyword with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    void DeleteKeyword(int id);

    /// <summary>
    /// Prunes the keywords by removing all the keywords not assigned to
    /// any work.
    /// </summary>
    void PruneKeywords();
}
