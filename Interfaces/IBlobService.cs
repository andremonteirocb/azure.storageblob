using Fundamentos.Azure.StorageBlob.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundamentos.Azure.StorageBlob.Interfaces
{
    public interface IBlobService
    {
        Task<FileCreated> Upload(FormFileBase64 file);
        Task<FileCreated> Upload(IFormFile file);
        Task<byte[]> GetFile(string fileName);
        Task<byte[]> GetZipFiles(IEnumerable<string> filesNames);
        Task<string> GetFileUrl(string fileName, bool generatePublicUrl = false);
        Task DeleteFile(string fileName);
    }
}
