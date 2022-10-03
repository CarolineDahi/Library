using Library.DataTransferObjects.Bill;
using Library.Main.BillRepository;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BillController : Controller
    {
        private readonly IBillRepository billRepository;

        public BillController(IBillRepository billRepository)
        {
            this.billRepository = billRepository;
        }

        [HttpPost]
        public async Task<IActionResult> GetBills(BillFilterDto filterDto)
            => await billRepository.GetAll(filterDto).ToJsonResultAsync();

        [HttpGet] 
        public async Task<IActionResult> GetById([Required] Guid id)
            => await billRepository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create(SetBillDto billDto)
            => await billRepository.Create(billDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await billRepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await billRepository.DeleteRange(ids).ToJsonResultAsync();   
    }
}
