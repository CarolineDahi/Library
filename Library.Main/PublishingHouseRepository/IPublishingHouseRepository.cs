using Library.DataTransferObjects.PublishingHouse;
using Library.SharedKernel.OperationResult;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Main.PublishingHouseRepository
{
    public interface IPublishingHouserepository
    {
        Task<OperationResult<IEnumerable<GetPublishingHouseDto>>> GetAll();
        Task<OperationResult<GetPublishingHouseDto>> GetById(Guid id);
        Task<OperationResult<GetPublishingHouseDto>> Create(SetPublishingHouseDto publishingHouseDto);
        Task<OperationResult<GetPublishingHouseDto>> Update(UpdatePublishingHouseDto publishingHouseDto);
        Task<OperationResult<bool>> Delete(Guid id);
        Task<OperationResult<bool>> DeleteRange(IEnumerable<Guid> ids);
    }
}
