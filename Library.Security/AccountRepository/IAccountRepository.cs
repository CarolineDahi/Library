using Library.DataTransferObjects.User;
using Library.SharedKernel.Enums;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Security.AccountRepository
{
    public interface IAccountRepository
    {
        Task<OperationResult<IEnumerable<GetUserDto>>> GetByType(UserType type);
        Task<OperationResult<GetAccountDto>> Login(LoginDto loginDto);
        Task<OperationResult<GetAccountDto>> Create(SetAccountDto accountDto);
        Task<OperationResult<GetAccountDto>> Update(UpdateAccountDto accountDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
