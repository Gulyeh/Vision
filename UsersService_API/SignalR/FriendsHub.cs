using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UsersService_API.Dtos;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.SignalR
{
    [Authorize]
    public class FriendsHub : Hub
    {
        private readonly IFriendsRepository friendsRepository;
        private readonly IUserRepository userRepository;
        private readonly Guid userId;
        public FriendsHub(IFriendsRepository friendsRepository, IUserRepository userRepository)
        {
            this.friendsRepository = friendsRepository;
            this.userRepository = userRepository;
            userId = Context.User != null ? Context.User.GetId() : Guid.Empty;
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task AcceptFriendRequest(Guid SenderId){
            if(!await friendsRepository.AcceptFriendRequest(userId, SenderId)) throw new HubException();

            await SendToUserOnlineData(SenderId, "NewFriend");

            var senderData = await userRepository.GetUserData(SenderId);
            await Clients.Caller.SendAsync("NewFriend", senderData);
        }

        public async Task DeclineFriendRequest(Guid SenderId){
            if(!await friendsRepository.DeclineFriendRequest(userId, SenderId)) throw new HubException();

            await SendToUserOnlineId(SenderId, "RequestDeclined");

            await Clients.Caller.SendAsync("RequestDeclined", SenderId);
        }

        public async Task DeleteFriend(Guid FriendToDelete){
            if(!await friendsRepository.DeleteFriend(userId, FriendToDelete)) throw new HubException();

            await SendToUserOnlineId(FriendToDelete, "FriendDeleted");

            await Clients.Caller.SendAsync("FriendDeleted", FriendToDelete);
        }
        
        public async Task SendFriendRequest(FriendRequestDto data){
            data.Sender = userId;
            if(!await friendsRepository.SendFriendRequest(data)) throw new HubException();

            await SendToUserOnlineData(data.Receiver, "NewFriendRequest");

            var requestedData = await userRepository.GetUserData(data.Receiver);
            await Clients.Caller.SendAsync("FriendRequestsPending", requestedData);
        }

        private async Task SendToUserOnlineData(Guid friendId, string connName){
            var connIds = await userRepository.CheckFriendIsOnline(friendId);
            if(connIds.Count > 0){
                var data = await userRepository.GetUserData(userId);
                await Clients.Users(connIds).SendAsync(connName, data);
            }
        }

        private async Task SendToUserOnlineId(Guid friendId, string connName){
            var connIds = await userRepository.CheckFriendIsOnline(friendId);
            if(connIds.Count > 0){
                await Clients.Users(connIds).SendAsync(connName, userId);
            }
        }
    }
}