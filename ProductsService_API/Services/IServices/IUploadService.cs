using CloudinaryDotNet.Actions;

namespace ProductsService_API.Services.IServices
{
    public interface IUploadService
    {
        Task<ImageUploadResult> UploadPhoto(byte[] file);
        Task<DeletionResult> DeletePhoto(string publicId);
    }
}