using Library.DataTransferObjects.Author;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.AuthorRepository
{
    public interface IAuthorRepository
    {
        Task<OperationResult<IEnumerable<GetAuthorDto>>> GetAll();
        Task<OperationResult<GetAuthorDto>> GetById(Guid id);
        Task<OperationResult<GetAuthorDto>> Create(SetAuthorDto authorDto);
        Task<OperationResult<GetAuthorDto>> Update(UpdateAuthorDto authorDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
