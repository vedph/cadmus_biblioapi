using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Keywords.
/// </summary>
/// <seealso cref="Controller" />
[Authorize]
[ApiController]
public sealed class KeywordController : Controller
{
    private readonly IBiblioRepository _repository;

    public KeywordController(IBiblioRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get the specified page of keywords.
    /// </summary>
    /// <param name="model">The keyword filter model.</param>
    /// <returns>Page.</returns>
    [HttpGet("api/keywords")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    public ActionResult<DataPage<Keyword>> GetKeywords(
        [FromQuery] KeywordFilterBindingModel model)
    {
        return Ok(_repository.GetKeywords(new KeywordFilter
        {
            PageNumber = model.PageNumber,
            PageSize = model.PageSize,
            Language = model.Language,
            Value = model.Value
        }));
    }

    /// <summary>
    /// Get the keyword with the specified ID.
    /// </summary>
    /// <param name="id">Keyword's ID.</param>
    /// <returns>Keyword.</returns>
    [HttpGet("api/keywords/{id}", Name = "GetKeyword")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<Keyword> GetKeyword([FromRoute] int id)
    {
        Keyword? keyword = _repository.GetKeyword(id);
        if (keyword == null) return NotFound();
        return Ok(keyword);
    }

    /// <summary>
    /// Add or update the specified keyword.
    /// </summary>
    /// <param name="model">The keyword.</param>
    [HttpPost("api/keywords")]
    [Produces("application/json")]
    [ProducesResponseType(201)]
    public IActionResult AddKeyword([FromBody] KeywordBindingModel model)
    {
        Keyword keyword = new()
        {
            Language = model.Language,
            Value = model.Value
        };
        int id = _repository.AddKeyword(keyword);
        return CreatedAtRoute("GetKeyword", new
        {
            id,
        }, keyword);
    }

    /// <summary>
    /// Deletes the specified keyword.
    /// </summary>
    /// <param name="id">The identifier.</param>
    [HttpDelete("api/keywords/{id}")]
    public void DeleteKeyword([FromRoute] int id)
    {
        _repository.DeleteKeyword(id);
    }

    /// <summary>
    /// Prunes the unused keywords.
    /// </summary>
    [Authorize(Roles = "admin,editor")]
    [HttpDelete("api/unused/keywords")]
    public void PruneKeywords()
    {
        _repository.PruneKeywords();
    }
}
