using Library.DataTransferObjects.Customer;
using Library.Main.CustomerRepository;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository customerRepository;

        public CustomerController(ICustomerRepository customerRepository)
        {
            this.customerRepository = customerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers()
            => await customerRepository.GetAll().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id)
            => await customerRepository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create(SetCustomerDto customerDto)
            => await customerRepository.Create(customerDto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(UpdateCustomerDto customerDto)
            => await customerRepository.Update(customerDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await customerRepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await customerRepository.DeleteRange(ids).ToJsonResultAsync();
    }
}
