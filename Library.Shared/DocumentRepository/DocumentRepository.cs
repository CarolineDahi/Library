using Library.Base;
using Library.DataTransferObjects.Document;
using Library.Models.Shared;
using Library.SharedKernel.ExtensionMethods;
using Library.SharedKernel.OperationResult;
using Library.SQL.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Shared.DocumentRepository
{
    public partial class DocumentRepository : LibraryRepository, IDocumentRepository
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public DocumentRepository(LibraryDBContext context, IHostingEnvironment hostingEnvironment) : base(context)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        public async Task<OperationResult<List<GetDocumentDto>>> Add(string foldername, List<IFormFile> files)
        {
            var operation = new OperationResult<List<GetDocumentDto>>();
            try
            {
                TryUploadFile(foldername, files, out List<string> paths, out List<string> names);
                List<Document> DocToAdd = new List<Document>();
                foreach (var path in paths)
                {
                    var document = new Document()
                    {
                        Name = names[paths.IndexOf(path)],
                        Path = path,
                        Type = ExtensionMethods.TypeOfDocument(files.First()),

                    };
                    DocToAdd.Add(document);
                }
                Context.AddRange(DocToAdd);
                await Context.SaveChangesAsync();

                operation.SetSuccess(DocToAdd.Select(d => new GetDocumentDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Path = d.Path,
                    Type = d.Type,
                }).ToList());
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<bool>> Delete(List<Guid> ids)
        {
            var operation = new OperationResult<bool>();
            using (var transaction = Context.Database.BeginTransaction())
            {
                try
                {
                    var docs = await Context.Documents.Where(d => ids.Contains(d.Id)).ToListAsync();
                    TryDeleteFile(docs.Select(d => d.Path).ToList());
                    Context.RemoveRange(docs);
                    await Context.SaveChangesAsync();
                    operation.SetSuccess(true);
                }
                catch (Exception ex)
                {
                    operation.SetException(ex);
                }
            }
            return operation;
        }

        public async Task<OperationResult<string>> Upload(string foldername, IFormFile file)
        {
            var operation = new OperationResult<string>();
            try
            {
                if (file == null)
                {
                    operation.SetSuccess("");
                }

                TryUploadFile(foldername, new List<IFormFile>() { file }, out List<string> paths, out List<string> names);

                operation.SetSuccess(paths.First());
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }

        public async Task<OperationResult<bool>> Remove(string path)
        {
            var operation = new OperationResult<bool>();

            try
            {
                TryDeleteFile(new List<string>() { path });

                operation.SetSuccess(true);
            }
            catch (Exception ex)
            {
                operation.SetException(ex);
            }
            return operation;
        }
    }
}
