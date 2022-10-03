using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Shared.DocumentRepository
{
    public partial class DocumentRepository
    {
        private bool TryUploadFile(string foldername, List<IFormFile> files, out List<string> paths, out List<string> names)
        {
            paths = new List<string>();
            names = new List<string>();
            try
            {
                if (files != null)
                {
                    var documentsDirectory = Path.Combine("wwwroot", "Documents", foldername);

                    if (!Directory.Exists(documentsDirectory))
                    {
                        Directory.CreateDirectory(documentsDirectory);
                    }
                    foreach (var file in files)
                    {
                        var path = Path.Combine("Documents", foldername, Guid.NewGuid().ToString() + "_" + file.FileName);
                        paths.Add(path);
                        names.Add(file.FileName);
                        string fileName = Path.Combine(hostingEnvironment.WebRootPath, path);
                        using (var fileStream = new FileStream(fileName, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private async Task<bool> TryDeleteFile(List<string> paths)
        {
            if (paths != null)
            {
                try
                {
                    foreach (var path in paths)
                    {
                        var pathFile = Path.Combine(hostingEnvironment.WebRootPath, path);
                        File.Delete(pathFile);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return true;
        }
    }
}
