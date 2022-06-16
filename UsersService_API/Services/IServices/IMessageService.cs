using UsersService_API.Dtos;

namespace UsersService_API.Services.IServices
{
    public interface IMessageService
    {
        Task<ResponseDto?> CheckUnreadMessages(string access_token, ICollection<Guid> FriendsList);
    }
}