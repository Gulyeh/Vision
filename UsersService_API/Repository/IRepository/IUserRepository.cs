using UsersService_API.Dtos;
using UsersService_API.Helpers;

namespace UsersService_API.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> ChangeStatus(Guid userId, Status status);
        Task<string> ChangePhoto(Guid userId, string base64);
        Task<bool> ChangeUserData(Guid userId, EditableUserDataDto data);
        Task<UserDataDto> GetUserData(Guid userId);
        Task<bool> UserOnline(Guid userId, string connectionId);
        Task<bool> UserOffline(Guid userId, string connectionId);
        Task<List<string>> GetUserFriendsOnline(Guid userId);
        Task<List<string>> CheckUserIsOnline(Guid userId, HubTypes hubType);
        Task<IEnumerable<GetUserDto>> FindUsers(string containsString, Guid userId);
        Task<IEnumerable<GetDetailedUsersDto>> FindDetailedUsers(string containsString);
        Task CreateUser(Guid userId);
        Task<ResponseDto> UserExists(Guid userId);
        Task<ResponseDto> IsUserBlocked(Guid userId, Guid user2Id);
        Task KickUser(Guid userId, string? reason = null);
        Task DeleteUser(Guid userId);
    }
}