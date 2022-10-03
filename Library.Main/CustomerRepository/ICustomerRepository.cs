using Library.DataTransferObjects.Customer;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task<OperationResult<IEnumerable<GetCustomerDto>>> GetAll();
        Task<OperationResult<GetCustomerDto>> GetById(Guid id);
        Task<OperationResult<GetCustomerDto>> Create(SetCustomerDto customerDto);
        Task<OperationResult<GetCustomerDto>> Update(UpdateCustomerDto customerDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
