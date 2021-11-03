using Fundamentos.Azure.StorageBlob.Interfaces;
using Fundamentos.Azure.StorageBlob.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Fundamentos.Azure.StorageBlob.Controllers
{
    [ApiController]
    [Route("api/storage")]
    public class StorageController : ControllerBase
    {
        private readonly IBlobService _blob;
        public StorageController(IBlobService blob)
        {
            _blob = blob;
        }

        [HttpPost("upload-formfile")]
        public async Task<ActionResult> UploadFormFile(IFormFile file)
        {
            var retorno = await _blob.Upload(file);
            return Ok(retorno);
        }

        [HttpPost("upload-base64")]
        public async Task<ActionResult> UploadBase64(FormFileBase64 file)
        {
            var retorno = await _blob.Upload(file);
            return Ok(retorno);
        }

        [HttpPost("obter-arquivos-zip")]
        public async Task<ActionResult> ObterArquivosZip(List<string> filesNames)
        {
            var retorno = await _blob.GetZipFiles(filesNames);
            return Ok(retorno);
        }

        [HttpGet("obter-arquivo")]
        public async Task<ActionResult> ObterArquivo(string fileName)
        {
            var retorno = await _blob.GetFile(fileName);
            return Ok(retorno);
        }

        [HttpGet("obter-arquivo-url")]
        public async Task<ActionResult> ObterArquivoUrl(string fileName)
        {
            var retorno = await _blob.GetFileUrl(fileName);
            return Ok(retorno);
        }

        [HttpDelete("remover-arquivo")]
        public async Task<ActionResult> RemoverArquivo(string fileName)
        {
            await _blob.DeleteFile(fileName);
            return Ok();
        }
    }
}
