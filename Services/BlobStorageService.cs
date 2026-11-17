
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AgriStreamIoT.Services
{
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private const string ContainerName = "agristream-images";

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Upload an image to Azure Blob Storage
        public async Task<string> UploadImageAsync(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                throw new ArgumentException("Please provide a valid image.");
            }

            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";

            BlobContainerClient containerClient =
                _blobServiceClient.GetBlobContainerClient(ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (Stream stream = image.OpenReadStream())
            {
                await blobClient.UploadAsync(
                    stream,
                    new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders
                        {
                            ContentType = image.ContentType
                        }
                    });
            }

            return blobClient.Uri.ToString();
        }

        // Get the URL of an uploaded image
        public async Task<string?> GetImageUrlAsync(string fileName)
        {
            BlobContainerClient containerClient =
                _blobServiceClient.GetBlobContainerClient(ContainerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                return blobClient.Uri.ToString();
            }

            return null;
        }

        // Delete an image from Azure Blob Storage
        public async Task<bool> DeleteImageAsync(string fileName)
        {
            BlobContainerClient containerClient =
                _blobServiceClient.GetBlobContainerClient(ContainerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            var response = await blobClient.DeleteIfExistsAsync();

            return response.Value;
        }
    }
}
