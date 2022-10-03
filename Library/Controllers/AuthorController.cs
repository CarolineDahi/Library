using Library.DataTransferObjects.Author;
using Library.Main.AuthorRepository;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository authorRepository;

        public AuthorController(IAuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAuthors()
            => await authorRepository.GetAll().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id)
            => await authorRepository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SetAuthorDto authorDto)
            => await authorRepository.Create(authorDto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateAuthorDto authorDto)
            => await authorRepository.Update(authorDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await authorRepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await authorRepository.DeleteRange(ids).ToJsonResultAsync();

    }
}
