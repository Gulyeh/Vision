using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace UsersService_API.Services.IServices
{
    public interface IUploadService
    {
        Task<ImageUploadResult> UploadPhoto(IFormFile file);
        Task<DeletionResult> DeletePhoto(string publicId);
    }
}