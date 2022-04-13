using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using UsersService_API.Helpers;
using UsersService_API.Services.IServices;

namespace UsersService_API.Services
{
    public class UploadService : IUploadService
    {
        private readonly Cloudinary cloudinary;

        public UploadService(IOptions<CloudinarySettings> options)
        {
            var cloudinaryAccount = new Account
            {
                ApiKey = options.Value.ApiKey,
                ApiSecret = options.Value.ApiSecret,
                Cloud = options.Value.CloudName
            };

            cloudinary = new Cloudinary(cloudinaryAccount);
        }

        public async Task<DeletionResult> DeletePhoto(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var results = await cloudinary.DestroyAsync(deleteParams);
            return results;
        }

        public async Task<ImageUploadResult> UploadPhoto(IFormFile file)
        {
            var uploadResults = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream)
                };

                uploadResults = await cloudinary.UploadAsync(uploadParams);
            }

            return uploadResults;
        }
    }
}