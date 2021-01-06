using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Mvc;

namespace Cadmus.Biblio.Api.Controllers
{
    /// <summary>
    /// Work type controller.
    /// </summary>
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
        public ActionResult<DataPage<WorkType>> GetTypes(
            [FromQuery] WorkTypeFilterBindingModel model)
        {
            return Ok(_repository.GetTypes(new WorkTypeFilter
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
        [HttpGet("api/work-types/{id}", Name = "GetType")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<WorkType> GetType([FromRoute] string id)
        {
            WorkType type = _repository.GetType(id);
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
        public IActionResult AddType([FromBody] WorkTypeBindingModel model)
        {
            WorkType type = new WorkType
            {
                Id = model.Id,
                Name = model.Name
            };
            _repository.AddType(type);
            return CreatedAtRoute("GetType", new
            {
                id = type.Id,
            }, type);
        }

        [HttpDelete("api/work-types/{id}")]
        public void DeleteType([FromRoute] string id)
        {
            _repository.DeleteType(id);
        }
    }
}
