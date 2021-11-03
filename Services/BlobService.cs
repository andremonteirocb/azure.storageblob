using Azure.Storage.Blobs;
using Fundamentos.Azure.StorageBlob.Interfaces;
using Fundamentos.Azure.StorageBlob.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Fundamentos.Azure.StorageBlob.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _blobContainerClient;
        public BlobService(BlobContainerClient blobContainerClient)
        {
            _blobContainerClient = blobContainerClient;
        }

        public async Task<FileCreated> Upload(IFormFile file)
        {
            using (Stream ms = file.OpenReadStream())
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var blob = _blobContainerClient.GetBlobClient(fileName);

                var response = await blob.UploadAsync(ms);
                if (response.GetRawResponse().Status == 201)
                {
                    return new FileCreated
                    {
                        FileName = fileName,
                        Url = blob.Uri.ToString(),
                    };
                }

                return null;
            }
        }
        public async Task<FileCreated> Upload(FormFileBase64 file)
        {
            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(file.Base64)))
            {
                var fileName = $"{Guid.NewGuid()}.{file.FileExtension}";
                var blob = _blobContainerClient.GetBlobClient(fileName);

                var response = await blob.UploadAsync(ms);
                if (response.GetRawResponse().Status == 201)
                {
                    return new FileCreated
                    {
                        FileName = fileName,
                        Url = blob.Uri.ToString(),
                    };
                }

                return null;
            }
        }
        public async Task<byte[]> GetFile(string fileName)
        {
            var blob = _blobContainerClient.GetBlobClient(fileName);
            if (blob.ExistsAsync().Result)
            {
                using (var ms = new MemoryStream())
                {
                    await blob.DownloadToAsync(ms);
                    return ms.ToArray();
                }
            }

            return null;
        }
        public async Task<byte[]> GetZipFiles(IEnumerable<string> filesNames)
        {
            var blobs = new List<BlobClient>();
            foreach (var fileName in filesNames)
                blobs.Add(_blobContainerClient.GetBlobClient(fileName));

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var blob in blobs)
                    {
                        if (blob.ExistsAsync().Result)
                        {
                            ZipArchiveEntry zipItem = zip.CreateEntry(blob.Name);

                            using (var ms = new MemoryStream())
                            {
                                await blob.DownloadToAsync(ms);

                                using (MemoryStream originalFileMemoryStream = new MemoryStream(ms.ToArray()))
                                {
                                    using (Stream entryStream = zipItem.Open())
                                    {
                                        originalFileMemoryStream.CopyTo(entryStream);
                                    }
                                }
                            }
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
        public Task<string> GetFileUrl(string fileName, bool generatePublicUrl = false)
        {
            var blob = _blobContainerClient.GetBlobClient(fileName);
            return Task.FromResult(blob.Uri.ToString());
        }
        public async Task DeleteFile(string fileName)
        {
            var blob = _blobContainerClient.GetBlobClient(fileName);
            await blob.DeleteAsync();
        }
    }
}
