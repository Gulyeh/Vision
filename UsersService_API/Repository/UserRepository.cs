using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;
using UsersService_API.Dtos;
using UsersService_API.Entites;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;
using UsersService_API.Statics;

namespace UsersService_API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IUploadService uploadService;
        private readonly ICacheService cacheService;
        private readonly ILogger<UserRepository> logger;
        private readonly IHubContext<UsersHub> usersHub;

        public UserRepository(ApplicationDbContext db, IMapper mapper, IUploadService uploadService,
            ICacheService cacheService, ILogger<UserRepository> logger, IHubContext<UsersHub> usersHub)
        {
            this.db = db;
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.cacheService = cacheService;
            this.logger = logger;
            this.usersHub = usersHub;
        }

        public Task<bool> IsUserBlocked(Guid senderId, Guid receiverId) => Task.FromResult(db.BlockedUsers.Any(x => (x.BlockedId == senderId && x.BlockerId == receiverId) || (x.BlockedId == receiverId && x.BlockerId == senderId)));

        public async Task<string> ChangePhoto(Guid userId, string base64)
        {
            string oldPhotoId;
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null) return string.Empty;

            var results = await uploadService.UploadPhoto(base64);
            if (results.Error is not null) return string.Empty;

            oldPhotoId = user.PhotoId;
            user.PhotoId = results.PublicId;
            user.PhotoUrl = results.SecureUrl.AbsoluteUri;

            if (await SaveChangesAsync())
            {
                if (!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);
                logger.LogInformation("User with ID: {userId} has changed photo successfully", userId);

                var userFriendsOnline = await GetUserFriendsOnline(userId);
                if (userFriendsOnline.Count > 0) await usersHub.Clients.Clients(userFriendsOnline).SendAsync("UserChangedPhoto", new ChangedUserPhotoDto(userId, user.PhotoUrl));

                return user.PhotoUrl;
            }

            logger.LogInformation("Could not change photo for User with ID: {userId}", userId);
            if (!string.IsNullOrEmpty(user.PhotoId)) await uploadService.DeletePhoto(user.PhotoId);
            return string.Empty;
        }

        public async Task<bool> ChangeStatus(Guid userId, Status status)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null) return false;

            user.Status = status;
            return await SaveChangesAsync("ChangeStatus");
        }

        public async Task<bool> ChangeUserData(Guid userId, EditableUserDataDto data)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null) return false;

            if (user.Username.ToLower().Equals(data.Username.ToLower()) && user.Description.ToLower().Equals(data.Description.ToLower())) return false;
            mapper.Map(data, user);
            return await SaveChangesAsync("ChangeUserData");
        }

        public async Task<UserDataDto> GetUserData(Guid userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null) return new UserDataDto();

            var userDto = mapper.Map<UserDataDto>(user);
            return userDto;
        }

        public async Task<bool> UserOffline(Guid userId, string connectionId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is not null)
            {
                var onlineuser = new OnlineUsersData(userId, connectionId, HubTypes.Users);
                await cacheService.TryRemoveFromCache(onlineuser);
                var cache = await cacheService.TryGetFromCache(HubTypes.Users);

                if (cache.Any(x => x.Key == userId)) return false;

                user.LastOnlineStatus = user.Status == Status.Offline ? Status.Online : user.Status;
                user.Status = Status.Offline;
                await SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UserOnline(Guid userId, string connectionId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is not null)
            {
                if (user.Status != Status.Offline)
                {
                    user.Status = Status.Offline;
                    await SaveChangesAsync();
                }

                user.Status = user.LastOnlineStatus;
                var onlineuser = new OnlineUsersData(userId, connectionId, HubTypes.Users);
                if (await SaveChangesAsync())
                {
                    if (await cacheService.TryAddToCache(onlineuser)) return true;
                }
            }
            return false;
        }

        public async Task<List<string>> GetUserFriendsOnline(Guid userId)
        {
            List<string> connList = new List<string>();
            var cachedOnline = await cacheService.TryGetFromCache(HubTypes.Users);
            var userFriends = await db.UsersFriends.Where(x => x.User1 == userId || x.User2 == userId).ToListAsync();
            foreach (var friend in userFriends)
            {
                Guid friendId;
                _ = friend.User1 != userId ? friendId = friend.User1 : friendId = friend.User2;

                if (cachedOnline.ContainsKey(friendId))
                {
                    var connectionIds = cachedOnline.GetValueOrDefault(friendId);
                    if (connectionIds is not null) connList.AddRange(connectionIds);
                }
            }
            return connList;
        }


        public async Task<List<string>> CheckUserIsOnline(Guid userId, HubTypes hubType)
        {
            List<string> connList = new List<string>();
            var cachedOnline = await cacheService.TryGetFromCache(hubType);
            if (cachedOnline.ContainsKey(userId))
            {
                var connectionIds = cachedOnline.GetValueOrDefault(userId);
                if (connectionIds is not null) connList.AddRange(connectionIds);
            }
            return connList;
        }

        public async Task<IEnumerable<GetUserDto>> FindUsers(string containsString, Guid userId)
        {
            var foundUsers = await db.Users.Where(x => x.Username.ToLower().Contains(containsString.ToLower()) && !x.IsDeletedAccount).OrderBy(x => x.Username).Take(25).ToListAsync();
            if (foundUsers.Any(x => x.UserId == userId)) foundUsers.Remove(foundUsers.First(x => x.UserId == userId));
            return mapper.Map<IEnumerable<GetUserDto>>(foundUsers);
        }


        public async Task<bool> UserExists(Guid userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null) return false;
            return true;
        }

        public async Task CreateUser(Guid userId)
        {
            var newUser = new Users();
            newUser.UserId = userId;

            await db.Users.AddAsync(newUser);
            await SaveChangesAsync("CreateUser");
        }

        private async Task<bool> SaveChangesAsync(string? methodName = null)
        {
            if (await db.SaveChangesAsync() > 0)
            {
                if (methodName != null && !string.IsNullOrEmpty(methodName)) logger.LogInformation("Saved data from {methodName} successfully", methodName);
                return true;
            }

            if (methodName != null && !string.IsNullOrEmpty(methodName)) logger.LogInformation("Could not save data from {methodName}", methodName);
            return false;
        }

        public async Task<IEnumerable<GetDetailedUsersDto>> FindDetailedUsers(string containsString)
        {
            List<Users> users = new();
            var tryGuidParse = Guid.TryParse(containsString, out Guid userId);

            if (tryGuidParse) users = await db.Users.Where(x => x.UserId == userId).OrderBy(x => x.Username).Take(500).ToListAsync();
            else users = await db.Users.Where(x => x.Username.ToLower().Contains(containsString.ToLower())).OrderBy(x => x.Username).Take(500).ToListAsync();

            return mapper.Map<IEnumerable<GetDetailedUsersDto>>(users);
        }

        public async Task KickUser(Guid userId, string? reason = null)
        {
            var cachedIds = await cacheService.TryGetFromCache(HubTypes.Users);
            if (cachedIds.Any(x => x.Key == userId)) await usersHub.Clients.Clients(cachedIds[(Guid)userId]).SendAsync("UserKicked", reason);
        }

        public async Task DeleteUser(Guid userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is null) return;

            user.IsDeletedAccount = true;
            user.Username = "Deleted Account";
            user.Description = string.Empty;
            user.PhotoUrl = StaticData.DefaultPhoto;

            if (await db.SaveChangesAsync() > 0)
            {
                await KickUser(userId);
                if (!string.IsNullOrWhiteSpace(user.PhotoId)) await uploadService.DeletePhoto(user.PhotoId);
            }
        }

        public async Task SendUserMessageNotification(Guid receiverId, Guid senderId)
        {
            var userExists = await UserExists(receiverId);
            if (userExists)
            {
                var connIds = await cacheService.TryGetFromCache(HubTypes.Users);
                if (connIds.ContainsKey(receiverId))
                {
                    var userIds = connIds.GetValueOrDefault(receiverId);
                    if (userIds is not null) await usersHub.Clients.Clients(userIds).SendAsync("ChatNotification", senderId);
                }
            }
        }

        public async Task<bool> BanUser(Guid userId)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == userId);
            if (user is not null)
            {
                user.IsBanned = true;
                if (await db.SaveChangesAsync() > 0) return true;
            }
            return false;
        }
    }
}