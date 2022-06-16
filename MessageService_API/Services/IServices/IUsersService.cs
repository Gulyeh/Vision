using MessageService_API.Dtos;

namespace MessageService_API.Services.IServices
{
    public interface IUsersService
    {
        Task<ResponseDto?> CheckUserExists(Guid userId, string Access_Token);
        Task<ResponseDto?> SendUserMessageNotification(Guid userId, Guid chatId, string Access_Token);
        Task<ResponseDto?> CheckIfUserIsBlocked(Guid userId, Guid user2Id, string Access_Token);
    }
}