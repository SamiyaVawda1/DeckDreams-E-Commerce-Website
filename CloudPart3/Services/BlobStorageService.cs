using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace CloudPart3.Services
{
    //MODIFY THIS
    public class BlobStorageService
    {
        //this service is to interact with azure blob storage
        private readonly BlobServiceClient _blobServiceClient;

        public BlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AzureStorage"); 
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        //replacing this method with the Azure function to upload a product image
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, overwrite: true);

            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string fileUrl, string containerName)
        {
            var blobName = GetBlobNameFromUrl(fileUrl);
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }

        private string GetBlobNameFromUrl(string fileUrl)
        {
            var uri = new System.Uri(fileUrl);
            return System.Web.HttpUtility.UrlDecode(Path.GetFileName(uri.LocalPath));
        }
    }
}
