using AutoMapper;
using GameAccessService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UsersService_API.Dtos;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;

namespace UsersService_API.SignalR
{
    [Authorize]
    public class FriendsHub : Hub
    {
        private readonly IFriendsRepository friendsRepository;
        private readonly IUserRepository userRepository;
        private readonly ILogger<FriendsHub> logger;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;

        public FriendsHub(IFriendsRepository friendsRepository, IUserRepository userRepository, ILogger<FriendsHub> logger, IMapper mapper, ICacheService cacheService)
        {
            this.cacheService = cacheService;
            this.friendsRepository = friendsRepository;
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
        }

        private Guid GetId()
        {
            return Context.User != null ? Context.User.GetId() : Guid.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            Guid userId = GetId();
            var token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
            await Clients.Caller.SendAsync("GetFriendsData", await friendsRepository.GetPendingRequests(userId),
                await friendsRepository.GetFriendRequests(userId),
                await friendsRepository.GetFriends(userId, token));
            await cacheService.TryAddToCache(new OnlineUsersData(userId, Context.ConnectionId, HubTypes.Friends));
            logger.LogInformation("User with ID: {userId} has connected to FriendsHub with ID: {connId}", userId, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Guid userId = GetId();
            await base.OnDisconnectedAsync(exception);
            await cacheService.TryRemoveFromCache(new OnlineUsersData(userId, Context.ConnectionId, HubTypes.Friends));
            logger.LogInformation("User with ID: {userId} has disconnected from FriendsHub", GetId());
        }

        public async Task ToggleBlockUser(Guid UserToToggleId)
        {
            var userId = GetId();
            (bool isSuccess, bool isBlocked) = await friendsRepository.ToggleBlock(userId, UserToToggleId);
            if (!isSuccess) return;

            await MultipleClientSource("ToggleBlockFriend", UserToToggleId, userId, isBlocked);
        }

        public async Task AcceptFriendRequest(Guid SenderId)
        {
            var userId = GetId();
            if (!await friendsRepository.AcceptFriendRequest(userId, SenderId)) return;

            await SendUserData_ToUserOnline<GetFriendsDto>(SenderId, "NewFriend");

            var senderData = await userRepository.GetUserData(SenderId);

            await MultipleClientSource("AcceptedFriendRequest", mapper.Map<GetFriendsDto>(senderData), userId);
        }

        public async Task DeclineFriendRequest(Guid userId)
        {
            var deleterId = GetId();
            if (!await friendsRepository.DeclineFriendRequest(userId, deleterId)) return;

            await SendIdToUserOnline(userId, "RequestDeclined");
            await MultipleClientSource("RequestDeclined", userId, deleterId);
        }

        public async Task DeleteFriend(Guid FriendToDelete)
        {
            var userId = GetId();
            if (!await friendsRepository.DeleteFriend(userId, FriendToDelete)) return;

            await SendIdToUserOnline(FriendToDelete, "FriendDeleted");
            await MultipleClientSource("FriendDeleted", FriendToDelete, userId);
        }

        public async Task SendFriendRequest(FriendRequestDto data)
        {
            data.Sender = GetId();
            if (!await friendsRepository.SendFriendRequest(data)) return;

            await SendUserData_ToUserOnline<GetUserDto>(data.Receiver, "NewFriendRequest");

            var requestedData = await userRepository.GetUserData(data.Receiver);
            await MultipleClientSource("FriendRequestsPending", mapper.Map<GetUserDto>(requestedData), data.Sender);
        }

        private async Task MultipleClientSource(string connName, object data, Guid userId, object? data2 = null)
        {
            var cachedIds = await cacheService.TryGetFromCache(HubTypes.Friends);
            if (cachedIds.Any(x => x.Key == userId))
            {
                if (cachedIds[userId].Count > 0)
                {
                    if (data2 is not null) await Clients.Clients(cachedIds[userId]).SendAsync(connName, data, data2);
                    else await Clients.Clients(cachedIds[userId]).SendAsync(connName, data);
                }
            }
        }

        private async Task SendIdToUserOnline(Guid userId, string connName)
        {
            var connIds = await userRepository.CheckUserIsOnline(userId, HubTypes.Friends);
            if (connIds.Count > 0) await Clients.Clients(connIds).SendAsync(connName, GetId());
        }

        private async Task SendUserData_ToUserOnline<T>(Guid userId, string connName) where T : class
        {
            var connIds = await userRepository.CheckUserIsOnline(userId, HubTypes.Friends);
            if (connIds.Count > 0)
            {
                var senderId = GetId();
                var data = await userRepository.GetUserData(senderId);
                var mapped = mapper.Map<T>(data);
                await Clients.Clients(connIds).SendAsync(connName, mapped);
            }
        }
    }
}