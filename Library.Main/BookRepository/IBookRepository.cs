using Library.DataTransferObjects.Book;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.BookRepository
{
    public interface IBookRepository
    {
        Task<OperationResult<IEnumerable<GetBookDto>>> GetAll();
        Task<OperationResult<GetBookDto>> GetById(Guid id);
        Task<OperationResult<GetBookDto>> Create(SetBookDto bookDto);
        Task<OperationResult<GetBookDto>> Update(UpdateBookDto bookDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
