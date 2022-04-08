using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersService_API.Dtos;

namespace UsersService_API.Repository.IRepository
{
    public interface IFriendsRepository
    {
        Task<ResponseDto> GetFriends(Guid userId);
        Task<ResponseDto> GetFriendRequests(Guid userId);
        Task<bool> SendFriendRequest(FriendRequestDto data);
        Task<bool> AcceptFriendRequest(Guid userId, Guid SenderId);
        Task<bool> DeclineFriendRequest(Guid userId, Guid SenderId);
        Task<bool> DeleteFriend(Guid userId, Guid ToDeleteUserId);
        Task<ResponseDto> GetPendingRequests(Guid userId);
    }
}