using UsersService_API.Dtos;

namespace UsersService_API.Repository.IRepository
{
    public interface IFriendsRepository
    {
        Task<IEnumerable<GetFriendsDto>> GetFriends(Guid userId);
        Task<IEnumerable<GetFriendRequestsDto>> GetFriendRequests(Guid userId);
        Task<bool> SendFriendRequest(FriendRequestDto data);
        Task<bool> AcceptFriendRequest(Guid userId, Guid SenderId);
        Task<bool> DeclineFriendRequest(Guid userId, Guid SenderId);
        Task<bool> DeleteFriend(Guid userId, Guid ToDeleteUserId);
        Task<IEnumerable<GetPendingRequestsDto>> GetPendingRequests(Guid userId);
        Task<(bool, bool)> ToggleBlock(Guid userId, Guid UserToBlockId);
    }
}