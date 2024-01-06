using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstractions.Storage.Azure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infastructure.Services.Storage.Azure
{
    public class AzureStorage : Storage, IAzureStorage
    {
        readonly BlobServiceClient _blobServiceClient;
        BlobContainerClient _containerClient;
        BlobClient _blobClient;

        public AzureStorage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Storage:Azure"]);
        }

        public async Task DeleteAsync(string containerName, string fileName)
        {
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);
            await _blobClient.DeleteAsync();
        }

        public List<string> GetFiles(string containerName)
        {
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _containerClient.GetBlobs().Select(x=> x.Name).ToList();
        }

        public bool HasFile(string containerName, string fileName)
        {
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = _containerClient.GetBlobClient(fileName);

            return blobClient.Exists();
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string containerName, IFormFileCollection files)
        {
            _containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await _containerClient.CreateIfNotExistsAsync();
            await _containerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<(string fileName, string path)> datas = new();

            foreach (var file in files)
            {
                string newFileName = await FileRenameAsync(containerName, file.Name, HasFile);
                BlobClient blobClient = _containerClient.GetBlobClient(newFileName);
                await blobClient.UploadAsync(file.OpenReadStream());

                datas.Add((file.Name, $"{containerName}/{newFileName}"));
            }

            return datas;
        }
    }
}
