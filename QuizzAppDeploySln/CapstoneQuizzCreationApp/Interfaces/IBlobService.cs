namespace CapstoneQuizzCreationApp.Interfaces
{
   public interface IBlobService
    {
        public Task<string> UploadImageAsync(IFormFile file, string blobName);

    }
}
