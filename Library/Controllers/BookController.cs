using Library.DataTransferObjects.Book;
using Library.Main.BookRepository;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BookController : Controller
    {
        private readonly IBookRepository bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            this.bookRepository = bookRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooks()
            => await bookRepository.GetAll().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id)
            => await bookRepository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] SetBookDto bookDto)
            => await bookRepository.Create(bookDto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] UpdateBookDto bookDto)
            => await bookRepository.Update(bookDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await bookRepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await bookRepository.DeleteRange(ids).ToJsonResultAsync();
    }
}
