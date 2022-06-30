using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IUsersService
    {
        Task<ResponseDto?> FindUser(string contains);
        Task<ResponseDto?> ChangePhoto(string image);
        Task<ResponseDto?> GetDetailedUsers(string containsString);
        Task<ResponseDto?> ChangeCurrency(Guid userId, int Amount);
    }
}
