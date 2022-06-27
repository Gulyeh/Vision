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
    public class UsersHub : Hub
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<UsersHub> logger;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;

        public UsersHub(IUserRepository userRepository, ILogger<UsersHub> logger, IMapper mapper, ICacheService cacheService)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        private Guid GetId()
        {
            return Context.User != null ? Context.User.GetId() : Guid.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            Guid userId = GetId();

            if (!await userRepository.UserOnline(userId, Context.ConnectionId))
            {
                await Clients.Caller.SendAsync("GetUserData", null);
                return;
            }

            var userData = await userRepository.GetUserData(userId);
            var cached = await cacheService.TryGetFromCache(HubTypes.Users);
            if (cached.Any(x => x.Key == userId))
            {
                if (userData.Status != Status.Invisible && cached[userId].Count == 1) await SendToFriendsOnline(mapper.Map<GetFriendsDto>(userData), "UserIsOnline");
            }

            await Clients.Caller.SendAsync("GetUserData", userData);
            logger.LogInformation("User with ID: {userId} has connected to UsersHub with ID: {connId}", userId, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Guid userId = GetId();
            if (!await userRepository.UserOffline(userId, Context.ConnectionId)) return;
            await SendToFriendsOnline(userId, "UserIsOffline");
            await base.OnDisconnectedAsync(exception);
            logger.LogInformation("User with ID: {userId} has disconnected from UsersHub", userId);
        }

        public async Task ChangeUserData(EditableUserDataDto data)
        {
            Guid userId = GetId();

            if (!await userRepository.ChangeUserData(userId, data))
            {
                logger.LogError("User with ID: {userId} could not change data", userId);
                return;
            }

            var newData = new ChangedUserDataDto(userId, data.Description, data.Username);
            await SendToFriendsOnline(newData, "UserChangedData");
            await CheckIfMultipleClients("ChangedData", newData, userId);
            logger.LogError("User with ID: {userId} changed name to: {name} and description to: {desc}", userId, data.Username, data.Description);
        }

        public async Task ChangeUserStatus(Status status)
        {
            Guid userId = GetId();

            if (!await userRepository.ChangeStatus(userId, status))
            {
                logger.LogError("User with ID: {userId} could not change status", userId);
                return;
            }

            var newStatus = new ChangedUserStatusDto(userId, status);
            await SendToFriendsOnline(newStatus, "UserChangedStatus");
            await CheckIfMultipleClients("ChangedStatus", newStatus, userId);
        }

        public async Task KickUser(KickUserDto data) {
            Guid userId = GetId();
            if(userId.Equals(data.UserId)) return;
            await CheckIfMultipleClients("UserKicked", data.Reason, data.UserId);
        }

        private async Task CheckIfMultipleClients(string connName, object data, Guid userId)
        {
            var cachedIds = await cacheService.TryGetFromCache(HubTypes.Users);
            if (cachedIds.Any(x => x.Key == userId))
            {
                if (cachedIds[userId].Count > 0) await Clients.Clients(cachedIds[userId]).SendAsync(connName, data);          
            }
        }

        private async Task SendToFriendsOnline(object data, string connName)
        {
            Guid userId = GetId();
            var userFriendsOnline = await userRepository.GetUserFriendsOnline(userId);
            if (userFriendsOnline.Count > 0) await Clients.Clients(userFriendsOnline).SendAsync(connName, data);
        }
    }
}