using Azure.Storage;
using Azure.Storage.Blobs;

namespace VUtor.Controllers
{
    public class AzureStorage
    {
        private readonly string _storageAccount = "vutorstorage";
        private readonly string _accessKey = "jUBuDRqAIwWkORa1XGHwfvsKlcF1Rg4mow+fu+172dmA29wkrCqfowiRlDtrLpjVsKF5YUnG63nO+AStZ0CKLg==";
        private readonly BlobServiceClient _blobServiceClient;

        public AzureStorage()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _accessKey);
            var blovUri = $"https://{_storageAccount}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blovUri), credential);
        }

        public Task ListBlobContainersAsync()
        {
            var containers = _blobServiceClient.GetBlobContainers();
            foreach(var container in containers)
            {
                Console.WriteLine(container.Name);
            }
            return Task.CompletedTask;
        }

        public async Task<List<Uri>> UploadFilesAsync(string folder, string filePath)
        {
            var blobUris = new List<Uri>();
            var blobContainer = _blobServiceClient.GetBlobContainerClient("user-resources");

            var blob = blobContainer.GetBlobClient(folder + "/" + filePath);

            await blob.UploadAsync(filePath, true);
            blobUris.Add(blob.Uri);

            return blobUris;
        }
    }
}
