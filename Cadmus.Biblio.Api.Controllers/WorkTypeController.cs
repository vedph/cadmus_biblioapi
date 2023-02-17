using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cadmus.Biblio.Api.Controllers;

/// <summary>
/// Work type controller.
/// </summary>
[Authorize]
[ApiController]
public sealed class WorkTypeController : Controller
{
    private readonly IBiblioRepository _repository;

    public WorkTypeController(IBiblioRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Get the specified page of work types.
    /// </summary>
    /// <param name="model">The filter model.</param>
    /// <returns>Page.</returns>
    [HttpGet("api/work-types")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    public ActionResult<DataPage<WorkType>> GetWorkTypes(
        [FromQuery] WorkTypeFilterBindingModel model)
    {
        return Ok(_repository.GetWorkTypes(new WorkTypeFilter
        {
            PageNumber = model.PageNumber,
            PageSize = model.PageSize,
            Name = model.Name
        }));
    }

    /// <summary>
    /// Get the work type with the specified ID.
    /// </summary>
    /// <param name="id">Type's ID.</param>
    /// <returns>Type.</returns>
    [HttpGet("api/work-types/{id}", Name = "GetWorkType")]
    [Produces("application/json")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public ActionResult<WorkType> GetWorkType([FromRoute] string id)
    {
        WorkType? type = _repository.GetWorkType(id);
        if (type == null) return NotFound();
        return Ok(type);
    }

    /// <summary>
    /// Add or update the specified work type.
    /// </summary>
    /// <param name="model">The work type.</param>
    [HttpPost("api/work-types")]
    [Produces("application/json")]
    [ProducesResponseType(201)]
    public IActionResult AddWorkType([FromBody] WorkTypeBindingModel model)
    {
        WorkType type = new()
        {
            Id = model.Id,
            Name = model.Name
        };
        _repository.AddWorkType(type);
        return CreatedAtRoute("GetWorkType", new
        {
            id = type.Id,
        }, type);
    }

    [HttpDelete("api/work-types/{id}")]
    public void DeleteWorkType([FromRoute] string id)
    {
        _repository.DeleteWorkType(id);
    }
}
