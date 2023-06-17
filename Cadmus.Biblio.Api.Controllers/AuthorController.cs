using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Authors.
/// </summary>
[Authorize]
[ApiController]
public sealed class AuthorController : Controller
{
    private readonly IBiblioRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorController"/>
    /// class.
    /// </summary>
    /// <param name="repository">The repository.</param>
    public AuthorController(IBiblioRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get the specified page of authors.
    /// </summary>
    /// <param name="model">The author filter model.</param>
    /// <returns>Page.</returns>
    [HttpGet("api/authors")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    public ActionResult<DataPage<Author>> GetAuthors(
        [FromQuery] AuthorFilterBindingModel model)
    {
        return Ok(_repository.GetAuthors(new AuthorFilter
        {
            PageNumber = model.PageNumber,
            PageSize = model.PageSize,
            Last = model.Last
        }));
    }

    /// <summary>
    /// Get the author with the specified ID.
    /// </summary>
    /// <param name="id">Author's ID.</param>
    /// <returns>Author.</returns>
    [HttpGet("api/authors/{id}", Name = "GetAuthor")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<Author> GetAuthor([FromRoute] Guid id)
    {
        Author? author = _repository.GetAuthor(id);
        if (author == null) return NotFound();
        return Ok(author);
    }

    /// <summary>
    /// Add or update the specified author.
    /// </summary>
    /// <param name="model">The author.</param>
    [HttpPost("api/authors")]
    [Produces("application/json")]
    [ProducesResponseType(201)]
    public IActionResult AddAuthor([FromBody] AuthorBindingModel model)
    {
        Author author = new()
        {
            Id = model.Id ?? Guid.Empty,
            First = model.First ?? "",
            Last = model.Last ?? "",
            Suffix = model.Suffix
        };
        _repository.AddAuthor(author);
        return CreatedAtRoute("GetAuthor", new
        {
            id = author.Id,
        }, author);
    }

    /// <summary>
    /// Deletes the author with the specified ID.
    /// </summary>
    /// <param name="id">The identifier.</param>
    [HttpDelete("api/authors/{id}")]
    public void DeleteAuthor([FromRoute] Guid id)
    {
        _repository.DeleteAuthor(id);
    }

    /// <summary>
    /// Prunes the unused authors.
    /// </summary>
    [Authorize(Roles = "admin,editor")]
    [HttpDelete("api/unused/authors")]
    public void PruneAuthors()
    {
        _repository.PruneAuthors();
    }
}
