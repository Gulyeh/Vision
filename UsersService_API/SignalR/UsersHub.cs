using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UsersService_API.Dtos;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.SignalR
{
    [Authorize]
    public class UsersHub : Hub
    {
        private readonly IUserRepository userRepository;
        private readonly Guid userId;
        public UsersHub(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            userId = Context.User != null ? Context.User.GetId() : Guid.Empty;
        }
        
        public override async Task OnConnectedAsync()
        {
            if(!await userRepository.UserOnline(userId, Context.ConnectionId)) throw new HubException();
            var userData = await userRepository.GetUserData(userId);
            await SendToFriendsOnline(userData, "UserIsOnline"); 

            await Clients.Caller.SendAsync("GetUserData", await userRepository.GetUserData(userId));        
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if(!await userRepository.UserOffline(userId, Context.ConnectionId)) throw new HubException();
            await SendToFriendsOnline(userId, "UserIsOffline");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task ChangeUserData(EditableUserDataDto data){
            if(!await userRepository.ChangeUserData(userId, data)) throw new HubException();
            var newData = new ChangedUserDataDto(userId, data.Description, data.Nickname);
            await SendToFriendsOnline(newData, "UserChangedData");         
        }

        public async Task ChangeUserStatus(Status status){
            if(await userRepository.ChangeStatus(userId, status)) throw new HubException();
            var newStatus = new ChangedUserStatusDto(userId, status);
            await SendToFriendsOnline(newStatus, "UserChangedStatus");
        }

        public async Task ChangeUserPhoto(IFormFile file){
            var photoUrl = await userRepository.ChangePhoto(userId, file);
            if(string.IsNullOrEmpty(photoUrl)) throw new HubException();
            var newPhoto = new ChangedUserPhotoDto(userId, photoUrl);
            await SendToFriendsOnline(newPhoto, "UserChangedPhoto");           
        }

        private async Task SendToFriendsOnline(object data, string connName){
            var userFriendsOnline = await userRepository.GetUserFriendsOnline(userId);   
            if(userFriendsOnline.Count > 0) await Clients.Users(userFriendsOnline).SendAsync(connName, data);
        }
    }
}