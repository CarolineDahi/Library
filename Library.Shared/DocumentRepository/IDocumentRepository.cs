using Library.DataTransferObjects.Document;
using Library.SharedKernel.OperationResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Shared.DocumentRepository
{
    public interface IDocumentRepository
    {
        Task<OperationResult<List<GetDocumentDto>>> Add(string foldername, List<IFormFile> files);
        Task<OperationResult<bool>> Delete(List<Guid> ids);
        Task<OperationResult<string>> Upload(string foldername, IFormFile file);
        Task<OperationResult<bool>> Remove(string path);
    }
}
