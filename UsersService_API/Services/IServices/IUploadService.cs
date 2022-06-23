using CloudinaryDotNet.Actions;

namespace UsersService_API.Services.IServices
{
    public interface IUploadService
    {
        Task<ImageUploadResult> UploadPhoto(string file);
        Task<DeletionResult> DeletePhoto(string publicId);
    }
}