using Cadmus.Biblio.Core;
using Fusi.Tools.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

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
