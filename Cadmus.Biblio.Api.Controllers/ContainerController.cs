using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadmus.Biblio.Api.Controllers
{
    /// <summary>
    /// Containers.
    /// </summary>
    /// <seealso cref="Controller" />
    [ApiController]
    public sealed class ContainerController : Controller
    {
        private readonly IBiblioRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerController"/>
        /// class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public ContainerController(IBiblioRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the specified page of containers.
        /// </summary>
        /// <param name="model">The container filter model.</param>
        /// <returns>Page.</returns>
        [HttpGet("api/containers")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult<DataPage<Container>> GetContainers(
            [FromQuery] WorkFilterBindingModel model)
        {
            return Ok(_repository.GetContainers(new WorkFilter
            {
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                IsMatchAnyEnabled = model.MatchAny,
                Type = model.Type,
                LastName = model.LastName,
                Language = model.Language,
                Title = model.Title,
                ContainerId = model.ContainerId.HasValue?
                    model.ContainerId.Value : Guid.Empty,
                Keyword = model.Keyword,
                YearPubMin = model.YearPubMin ?? 0,
                YearPubMax = model.YearPubMax ?? 0,
                Key = model.Key
            }));
        }

        /// <summary>
        /// Get the container with the specified ID.
        /// </summary>
        /// <param name="id">Container's ID.</param>
        /// <returns>Container.</returns>
        [HttpGet("api/containers/{id}", Name = "GetContainer")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Container> GetContainer([FromRoute] Guid id)
        {
            Container container = _repository.GetContainer(id);
            if (container == null) return NotFound();
            return Ok(container);
        }

        /// <summary>
        /// Add or update the specified container.
        /// </summary>
        /// <param name="model">The container.</param>
        [HttpPost("api/containers")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        public IActionResult AddContainer([FromBody] ContainerBindingModel model)
        {
            Container container = new Container
            {
                Id = model.Id ?? Guid.Empty,
                Key = model.Key,
                Authors = model.Authors?.Count > 0
                    ? model.Authors.ConvertAll(m => ModelHelper.GetAuthor(m))
                    : null,
                Type = model.Type,
                Title = model.Title,
                Language = model.Language,
                Edition = model.Edition ?? 0,
                Publisher = model.Publisher,
                YearPub = model.YearPub ?? 0,
                PlacePub = model.PlacePub,
                Location = model.Location,
                AccessDate = model.AccessDate,
                Note = model.Note,
                Keywords = model.Keywords?.Count > 0
                    ? model.Keywords.ConvertAll(m => ModelHelper.GetKeyword(m))
                    : null,
                Number = model.Number
            };
            _repository.AddContainer(container);
            return CreatedAtRoute("GetContainer", new
            {
                id = container.Id,
            }, container);
        }

        /// <summary>
        /// Deletes the container with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpDelete("api/containers/{id}")]
        public void DeleteContainer([FromRoute] Guid id)
        {
            _repository.DeleteContainer(id);
        }
    }
}
