using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersService_API.Dtos;
using UsersService_API.Helpers;

namespace UsersService_API.Repository.IRepository
{
    public interface IUserRepository
    {
        Task<bool> ChangeStatus(Guid userId, Status status);
        Task<string> ChangePhoto(Guid userId, IFormFile file);
        Task<bool> ChangeUserData(Guid userId, EditableUserDataDto data);
        Task<UserDataDto> GetUserData(Guid userId);
        Task<bool> UserOnline(Guid userId, string connectionId);
        Task<bool> UserOffline(Guid userId, string connectionId);
        Task<List<string>> GetUserFriendsOnline(Guid userId);
        Task<List<string>> CheckFriendIsOnline(Guid friendId);
    }
}