using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IUsersService
    {
        Task<ResponseDto?> FindUser(string contains);
        Task<ResponseDto?> ChangePhoto(string image);
    }
}
