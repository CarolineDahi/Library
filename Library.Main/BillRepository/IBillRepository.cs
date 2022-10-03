using Library.DataTransferObjects.Bill;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.BillRepository
{
    public interface IBillRepository
    {
        Task<OperationResult<IEnumerable<GetBillDto>>> GetAll(BillFilterDto filterDto);
        Task<OperationResult<GetBillDetailsDto>> GetById(Guid id);
        Task<OperationResult<GetBillDto>> Create(SetBillDto billDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
