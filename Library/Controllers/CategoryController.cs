using Library.DataTransferObjects.Category;
using Library.Shared.CategoryRepository;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
            => await categoryRepository.GetAll().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id)
            => await categoryRepository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create(SetCategoryDto categoryDto)
            => await categoryRepository.Create(categoryDto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCategoryDto categoryDto)
            => await categoryRepository.Update(categoryDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await categoryRepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await categoryRepository.DeleteRange(ids).ToJsonResultAsync();
    }
}
