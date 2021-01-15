using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadmus.Biblio.Api.Controllers
{
    /// <summary>
    /// Works.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Authorize]
    [ApiController]
    public sealed class WorkController : Controller
    {
        private readonly IBiblioRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkController"/>
        /// class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        public WorkController(IBiblioRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get the specified page of works.
        /// </summary>
        /// <param name="model">The work filter model.</param>
        /// <returns>Page.</returns>
        [HttpGet("api/works")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult<DataPage<Work>> GetWorks(
            [FromQuery] WorkFilterBindingModel model)
        {
            return Ok(_repository.GetWorks(
                ModelHelper.GetWorkFilter(model)));
        }

        /// <summary>
        /// Get the work with the specified ID.
        /// </summary>
        /// <param name="id">Work's ID.</param>
        /// <returns>Work.</returns>
        [HttpGet("api/works/{id}", Name = "GetWork")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Work> GetWork([FromRoute] Guid id)
        {
            Work work = _repository.GetWork(id);
            if (work == null) return NotFound();
            return Ok(work);
        }

        /// <summary>
        /// Add or update the specified work.
        /// </summary>
        /// <param name="model">The work.</param>
        [HttpPost("api/works")]
        [Produces("application/json")]
        [ProducesResponseType(201)]
        public IActionResult AddWork([FromBody] WorkBindingModel model)
        {
            Work work = new Work
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
                FirstPage = model.FirstPage ?? 0,
                LastPage = model.LastPage ?? 0,
                Location = model.Location,
                AccessDate = model.AccessDate,
                Note = model.Note,
                Keywords = model.Keywords?.Count > 0
                    ? model.Keywords.ConvertAll(m => ModelHelper.GetKeyword(m))
                    : null
            };
            _repository.AddWork(work);
            return CreatedAtRoute("GetWork", new
            {
                id = work.Id,
            }, work);
        }

        /// <summary>
        /// Deletes the work with the specified ID.
        /// </summary>
        /// <param name="id">The identifier.</param>
        [HttpDelete("api/works/{id}")]
        public void DeleteWork([FromRoute] Guid id)
        {
            _repository.DeleteWork(id);
        }
    }
}
