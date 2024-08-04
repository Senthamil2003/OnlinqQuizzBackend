using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CapstoneQuizzCreationApp.Interfaces;

namespace CapstoneQuizzCreationApp.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobService(string containerName,string secretString)
        {
            string connectionString = secretString;
            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = containerName;
        }


        public async Task<string> UploadImageAsync(IFormFile file, string blobName)
        {
            try
            {
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                var contentType = file.ContentType;

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error uploading image: {ex.Message}");
                throw;
            }
        }





    }
}
