using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;
using UsersService_API.Dtos;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;

namespace UsersService_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IUploadService uploadService;
        private readonly ICacheService cacheService;

        public UserRepository(ApplicationDbContext db, IMapper mapper, IUploadService uploadService, ICacheService cacheService)
        {
            this.db = db;
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.cacheService = cacheService;
        }

        public async Task<string> ChangePhoto(Guid userId, IFormFile file)
        {
            string oldPhotoId;
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if(user is null) return string.Empty;

            var results = await uploadService.UploadPhoto(file);
            if(results.Error is not null) return string.Empty;

            oldPhotoId = user.PhotoId;
            user.PhotoId = results.PublicId;
            user.PhotoUrl = results.SecureUrl.AbsoluteUri;   

            if(await SaveChangesAsync()){
                if(!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);
                return user.PhotoUrl;
            }     

            if(!string.IsNullOrEmpty(user.PhotoId)) await uploadService.DeletePhoto(user.PhotoId);
            return string.Empty;
        }

        public async Task<bool> ChangeStatus(Guid userId, Status status)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if(user is null) return false;

            user.Status = status;
            return await SaveChangesAsync();  
        }

        public async Task<bool> ChangeUserData(Guid userId, EditableUserDataDto data)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if(user is null) return false;
            mapper.Map(data, user);      
            return await SaveChangesAsync();  
        }

        public async Task<UserDataDto> GetUserData(Guid userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if(user is null) return new UserDataDto();
            return new UserDataDto(){ 
                UserId = userId,
                Nickname = user.Nickname,
                Description = user.Description,
                PhotoUrl = user.PhotoUrl,
                Status = user.LastOnlineStatus
             };
        }

        public async Task<bool> UserOffline(Guid userId, string connectionId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if(user is not null){
                var onlineuser = new OnlineUsersData(userId, connectionId);
                if(await cacheService.TryRemoveFromCache(onlineuser)){
                    user.LastOnlineStatus = user.Status;
                    user.Status = Status.Offline;
                    if(await SaveChangesAsync()){
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> UserOnline(Guid userId, string connectionId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if(user is not null){
                user.Status = user.LastOnlineStatus;
                var onlineuser = new OnlineUsersData(userId, connectionId);
                if(await SaveChangesAsync()) {
                    if(await cacheService.TryAddToCache(onlineuser)) return true;
                }
            }
            return false;
        }

        public async Task<List<string>> GetUserFriendsOnline(Guid userId){
            List<string> connList = new List<string>();
            var cachedOnline = await cacheService.TryGetFromCache();
            var userFriends = await db.UsersFriends.Where(x => x.User1 == userId || x.User2 == userId).ToListAsync();
            foreach(var friend in userFriends){
                Guid friendId;
                _ = friend.User1 != userId ? friendId = friend.User1 : friendId = friend.User2;

                if(cachedOnline.ContainsKey(friendId)){
                    var connectionIds = cachedOnline.GetValueOrDefault(friendId);
                    if(connectionIds is not null) connList.AddRange(connectionIds);
                }
            }
            return connList;
        }

        public async Task<List<string>> CheckFriendIsOnline(Guid friendId)
        {
            List<string> connList = new List<string>();
            var cachedOnline = await cacheService.TryGetFromCache();
            if(cachedOnline.ContainsKey(friendId)){
                    var connectionIds = cachedOnline.GetValueOrDefault(friendId);
                    if(connectionIds is not null) connList.AddRange(connectionIds);
            }
            return connList;
        }

        private async Task<bool> SaveChangesAsync(){
            if(await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}