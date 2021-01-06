using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Cadmus.Biblio.Api.Controllers
{
    [ApiController]
    public sealed class AuthorController : Controller
    {
        private readonly IBiblioRepository _repository;

        public AuthorController(IBiblioRepository repository)
        {
            _repository = repository;
        }

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

        [HttpGet("api/authors/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public ActionResult<Author> GetAuthor([FromRoute] Guid id)
        {
            Author author = _repository.GetAuthor(id);
            if (author == null) return NotFound();
            return Ok(author);
        }
    }
}
