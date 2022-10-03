using Library.DataTransferObjects.Category;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Shared.CategoryRepository
{
    public interface ICategoryRepository
    {
        Task<OperationResult<IEnumerable<GetCategoryDto>>> GetAll();
        Task<OperationResult<GetCategoryDto>> GetById(Guid id);
        Task<OperationResult<GetCategoryDto>> Create(SetCategoryDto categoryDto);
        Task<OperationResult<GetCategoryDto>> Update(UpdateCategoryDto categoryDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
