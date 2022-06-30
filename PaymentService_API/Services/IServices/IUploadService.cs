using CloudinaryDotNet.Actions;

namespace PaymentService_API.Services.IServices
{
    public interface IUploadService
    {
        Task<ImageUploadResult> UploadPhoto(string file);
        Task<DeletionResult> DeletePhoto(string publicId);
    }
}