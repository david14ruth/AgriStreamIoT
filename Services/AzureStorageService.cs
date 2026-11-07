
using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;

namespace AgriStreamIoT.Services
{
    public class AzureStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly TableServiceClient _tableServiceClient;
        private readonly QueueServiceClient _queueServiceClient;

        public AzureStorageService(IConfiguration configuration)
        {
            string connectionString =
                configuration.GetConnectionString("AzureStorage")
                ?? throw new InvalidOperationException(
                    "Azure Storage connection string is missing.");

            _blobServiceClient =
                new BlobServiceClient(connectionString);

            _tableServiceClient =
                new TableServiceClient(connectionString);

            _queueServiceClient =
                new QueueServiceClient(connectionString);
        }

        public async Task InitializeStorageAsync()
        {
            var blobContainer =
                _blobServiceClient.GetBlobContainerClient(
                    "agristream-images");

            await blobContainer.CreateIfNotExistsAsync();

            var telemetryTable =
                _tableServiceClient.GetTableClient(
                    "AgriTelemetry");

            await telemetryTable.CreateIfNotExistsAsync();

            var auditTable =
                _tableServiceClient.GetTableClient(
                    "IrrigationAudit");

            await auditTable.CreateIfNotExistsAsync();

            var irrigationQueue =
                _queueServiceClient.GetQueueClient(
                    "irrigation-commands");

            await irrigationQueue.CreateIfNotExistsAsync();
        }

        // Upload files to Azure Blob Storage
        public async Task<string> UploadBlobAsync(
            Stream fileStream,
            string fileName,
            string contentType)
        {
            var container =
                _blobServiceClient.GetBlobContainerClient(
                    "agristream-images");

            await container.CreateIfNotExistsAsync();

            var blob = container.GetBlobClient(fileName);

            await blob.UploadAsync(
                fileStream,
                new BlobHttpHeaders
                {
                    ContentType = contentType
                });

            return blob.Name;
        }
    }
}
