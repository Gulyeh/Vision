using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;
using UsersService_API.Dtos;
using UsersService_API.Entites;
using UsersService_API.Helpers;
using UsersService_API.Messages;
using UsersService_API.RabbitMQSender;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;

namespace UsersService_API.Repository
{
    public class FriendsRepository : IFriendsRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IRabbitMQSender rabbitMQSender;

        public FriendsRepository(ApplicationDbContext db, IMapper mapper, IRabbitMQSender rabbitMQSender)
        {
            this.db = db;
            this.mapper = mapper;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<bool> AcceptFriendRequest(Guid userId, Guid SenderId)
        {
            var findRequest = await db.FriendRequests.FirstOrDefaultAsync(x => x.Sender == SenderId && x.Receiver == userId);
            if(findRequest is null) return false;

            db.FriendRequests.Remove(findRequest);

            var newFriends = new UsersFriends(){
                User1 = SenderId,
                User2 = userId,
                FriendsSince = DateTime.Now
           };
           
            await db.UsersFriends.AddAsync(newFriends);

            return await SaveChangesAsyncBool();
        }

        public async Task<bool> DeclineFriendRequest(Guid userId, Guid SenderId)
        {
            var findRequest = await db.FriendRequests.FirstOrDefaultAsync(x => x.Sender == SenderId && x.Receiver == userId);
            if(findRequest is null) return false;

            db.FriendRequests.Remove(findRequest);
            return await SaveChangesAsyncBool();
        }

        public async Task<bool> DeleteFriend(Guid userId, Guid ToDeleteUserId)
        {
            var findFriends = await db.UsersFriends.FirstOrDefaultAsync(x => (x.User1 == userId && x.User2 == ToDeleteUserId) 
            || (x.User1 == ToDeleteUserId && x.User2 == userId));
            if(findFriends is null) return false;

            db.UsersFriends.Remove(findFriends);
            if(await SaveChangesAsyncBool()){
                rabbitMQSender.SendMessage(new DeleteChat { User1 = userId, User2 = ToDeleteUserId }, "DeleteChatQueue");
                return true;
            }
            return false;
        }

        public async Task<ResponseDto> GetFriends(Guid userId)
        {
            var findFriends = await db.UsersFriends.Where(x => x.User1 == userId || x.User2 == userId).ToListAsync();    
            List<UserDataDto> friendList = new List<UserDataDto>();

            foreach(var friend in findFriends){
                Guid User;
                _ = friend.User1 != userId ? User = friend.User1 : User = friend.User2;
                var data = await db.Users.FirstOrDefaultAsync(x => x.UserId == User);

                if(data is not null){
                    var friendDto = new UserDataDto(){
                        UserId = data.UserId,
                        Nickname = data.Nickname,
                        PhotoUrl = data.PhotoUrl,
                        Status = data.Status,
                        Description = data.Description
                    };
                    friendList.Add(friendDto);
                }
            }

            return new ResponseDto(true, StatusCodes.Status200OK, findFriends);
        }

        public async Task<bool> SendFriendRequest(FriendRequestDto data)
        {
            var mapped = mapper.Map<FriendRequests>(data);
            await db.FriendRequests.AddAsync(mapped);

            return await SaveChangesAsyncBool();
        }

        public async Task<ResponseDto> GetFriendRequests(Guid userId)
        {
            var findRequests = await db.FriendRequests.Where(x => x.Receiver == userId).ToListAsync();
            List<GetFriendRequestsDto> requests = new List<GetFriendRequestsDto>();

            foreach(var data in findRequests){
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.Sender);
                if(user is not null){
                    var request = new GetFriendRequestsDto(){
                        UserId = user.UserId,
                        Nickname = user.Nickname,
                        PhotoUrl = user.PhotoUrl,
                        Status = user.Status
                    };
                    requests.Add(request);
                }
            }

            return new ResponseDto(true, StatusCodes.Status200OK, findRequests);
        }

        public async Task<ResponseDto> GetPendingRequests(Guid userId)
        {
            var findSentRequests = await db.FriendRequests.Where(x => x.Sender == userId).ToListAsync();
            List<GetFriendRequestsDto> requests = new List<GetFriendRequestsDto>();

            foreach(var data in findSentRequests){
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.Receiver);
                if(user is not null){
                    var request = new GetFriendRequestsDto(){
                        UserId = user.UserId,
                        Nickname = user.Nickname,
                        PhotoUrl = user.PhotoUrl,
                        Status = user.Status
                    };
                    requests.Add(request);
                }
            }
      
            return new ResponseDto(true, StatusCodes.Status200OK, findSentRequests);
        }

        private async Task<bool> SaveChangesAsyncBool(){
            if(await db.SaveChangesAsync() > 0) {
                return true;
            }
            return false;
        }

        private async Task<ResponseDto> SaveChangesAsync(string message, string errormessage){
            if(await db.SaveChangesAsync() > 0) {
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { message });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { errormessage });
        }

    }
}