using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using PaymentService_API.Helpers;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Services
{
    public class UploadService : IUploadService
    {
        private readonly Cloudinary cloudinary;
        private readonly ILogger<UploadService> logger;

        public UploadService(IOptions<CloudinarySettings> options, ILogger<UploadService> logger)
        {
            var cloudinaryAccount = new Account
            {
                ApiKey = options.Value.ApiKey,
                ApiSecret = options.Value.ApiSecret,
                Cloud = options.Value.CloudName
            };

            cloudinary = new Cloudinary(cloudinaryAccount);
            this.logger = logger;
        }

        public async Task<DeletionResult> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var results = await cloudinary.DestroyAsync(deleteParams);
            if (results.Error is null) logger.LogInformation("Deleted photo with ID: {Id} successfully", publicId);
            else logger.LogError("Could not delete photo with ID: {Id}", publicId);
            return results;
        }

        public async Task<ImageUploadResult> UploadPhoto(byte[] file)
        {
            var uploadResults = new ImageUploadResult();
            if (file is not null)
            {
                var guid = Guid.NewGuid().ToString().Replace("-", "");
                using var stream = new MemoryStream(file);
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(guid, stream)
                };

                uploadResults = await cloudinary.UploadAsync(uploadParams);
            }

            if (uploadResults.Error is null && uploadResults.StatusCode == System.Net.HttpStatusCode.OK) logger.LogInformation("Uploaded photo successfully");
            else if (uploadResults.Error is not null) logger.LogError("Could not upload photo");

            return uploadResults;
        }
    }
}