using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GamesDataService_API.Helpers;
using GamesDataService_API.Services.IServices;
using Microsoft.Extensions.Options;

namespace GamesDataService_API.Services
{
    public class UploadService : IUploadService
    {
        private readonly Cloudinary cloudinary;

        public UploadService(IOptions<CloudinarySettings> options)
        {
            var cloudinaryAccount = new Account{
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
            if(file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams{
                    File = new FileDescription(file.FileName, stream)
                };

                uploadResults = await cloudinary.UploadAsync(uploadParams);
            }

            return uploadResults;
        }
    }
}