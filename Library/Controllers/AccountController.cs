using Library.DataTransferObjects.User;
using Library.Security.AccountRepository;
using Library.SharedKernel.Enums;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly IAccountRepository accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetByType([Required] UserType type)
            => await accountRepository.GetByType(type).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
            => await accountRepository.Login(loginDto).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create(SetAccountDto accountDto)
            => await accountRepository.Create(accountDto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(UpdateAccountDto accountDto)
            => await accountRepository.Update(accountDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await accountRepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await accountRepository.DeleteRange(ids).ToJsonResultAsync();
    }
}
