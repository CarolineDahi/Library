using Library.DataTransferObjects.PublishingHouse;
using Library.Main.PublishingHouseRepository;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PublishingHouseController : Controller
    {
        private readonly IPublishingHouserepository publishingHouserepository;

        public PublishingHouseController(IPublishingHouserepository publishingHouserepository)
        {
            this.publishingHouserepository = publishingHouserepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublishingHouses()
            => await publishingHouserepository.GetAll().ToJsonResultAsync();

        [HttpGet]
        public async Task<IActionResult> GetById([Required] Guid id)
            => await publishingHouserepository.GetById(id).ToJsonResultAsync();

        [HttpPost]
        public async Task<IActionResult> Create(SetPublishingHouseDto publishingHouseDto)
            => await publishingHouserepository.Create(publishingHouseDto).ToJsonResultAsync();

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePublishingHouseDto publishingHouseDto)
            => await publishingHouserepository.Update(publishingHouseDto).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> Delete([Required] Guid id)
            => await publishingHouserepository.Delete(id).ToJsonResultAsync();

        [HttpDelete]
        public async Task<IActionResult> DeleteRange(IEnumerable<Guid> ids)
            => await publishingHouserepository.DeleteRange(ids).ToJsonResultAsync();
    }
}
