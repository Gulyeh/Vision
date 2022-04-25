using AutoMapper;
using Microsoft.EntityFrameworkCore;
using UsersService_API.DbContexts;
using UsersService_API.Dtos;
using UsersService_API.Entites;
using UsersService_API.Messages;
using UsersService_API.RabbitMQSender;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.Repository
{
    public class FriendsRepository : IFriendsRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<FriendsRepository> logger;

        public FriendsRepository(ApplicationDbContext db, IMapper mapper, IRabbitMQSender rabbitMQSender, ILogger<FriendsRepository> logger)
        {
            this.db = db;
            this.mapper = mapper;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
        }

        public async Task<bool> AcceptFriendRequest(Guid userId, Guid SenderId)
        {
            var findRequest = await db.FriendRequests.FirstOrDefaultAsync(x => x.Sender == SenderId && x.Receiver == userId);
            if (findRequest is null) return false;

            db.FriendRequests.Remove(findRequest);

            var newFriends = new UsersFriends();       
            newFriends.User1 = SenderId;
            newFriends.User2 = userId;

            await db.UsersFriends.AddAsync(newFriends);

            return await SaveChangesAsync("AcceptFriendRequest");
        }

        public async Task<bool> DeclineFriendRequest(Guid userId, Guid SenderId)
        {
            var findRequest = await db.FriendRequests.FirstOrDefaultAsync(x => x.Sender == SenderId && x.Receiver == userId);
            if (findRequest is null) return false;

            db.FriendRequests.Remove(findRequest);
            return await SaveChangesAsync("DeclineFriendRequest");
        }

        public async Task<bool> DeleteFriend(Guid userId, Guid ToDeleteUserId)
        {
            var findFriends = await db.UsersFriends.FirstOrDefaultAsync(x => (x.User1 == userId && x.User2 == ToDeleteUserId)
            || (x.User1 == ToDeleteUserId && x.User2 == userId));
            if (findFriends is null) return false;

            db.UsersFriends.Remove(findFriends);
            if (await SaveChangesAsync())
            {
                rabbitMQSender.SendMessage(new DeleteChat() { User1 = userId, User2 = ToDeleteUserId }, "DeleteChatQueue");
                logger.LogInformation("User with ID: {userId} has deleted User with ID: {deletedUserId} from friendlist", userId, ToDeleteUserId);
                return true;
            }
            logger.LogInformation("Could not delete User with ID: {deleteUserId} from User with ID: {userId} friendlist", ToDeleteUserId, userId);
            return false;
        }

        public async Task<ResponseDto> GetFriends(Guid userId)
        {
            var findFriends = await db.UsersFriends.Where(x => x.User1 == userId || x.User2 == userId).ToListAsync();
            List<UserDataDto> friendList = new List<UserDataDto>();

            foreach (var friend in findFriends)
            {
                Guid User;
                _ = friend.User1 != userId ? User = friend.User1 : User = friend.User2;
                var data = await db.Users.FirstOrDefaultAsync(x => x.UserId == User);

                if (data is not null)
                {
                    var friendDto = mapper.Map<UserDataDto>(data);
                    friendList.Add(friendDto);
                }
            }

            return new ResponseDto(true, StatusCodes.Status200OK, friendList);
        }

        public async Task<bool> SendFriendRequest(FriendRequestDto data)
        {
            var mapped = mapper.Map<FriendRequests>(data);
            await db.FriendRequests.AddAsync(mapped);

            return await SaveChangesAsync("SendFriendRequest");
        }

        public async Task<ResponseDto> GetFriendRequests(Guid userId)
        {
            var findRequests = await db.FriendRequests.Where(x => x.Receiver == userId).ToListAsync();
            List<GetFriendRequestsDto> requests = new List<GetFriendRequestsDto>();

            foreach (var data in findRequests)
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.Sender);
                if (user is not null)
                {
                    var request = mapper.Map<GetFriendRequestsDto>(user);             
                    requests.Add(request);
                }
            }

            return new ResponseDto(true, StatusCodes.Status200OK, findRequests);
        }

        public async Task<ResponseDto> GetPendingRequests(Guid userId)
        {
            var findSentRequests = await db.FriendRequests.Where(x => x.Sender == userId).ToListAsync();
            List<GetFriendRequestsDto> requests = new List<GetFriendRequestsDto>();

            foreach (var data in findSentRequests)
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.Receiver);
                if (user is not null)
                {
                    var request = mapper.Map<GetFriendRequestsDto>(user);             
                    requests.Add(request);
                }
            }

            return new ResponseDto(true, StatusCodes.Status200OK, findSentRequests);
        }

        private async Task<bool> SaveChangesAsync(string? methodName = null)
        {
            if (await db.SaveChangesAsync() > 0) {
                if(methodName != null && !string.IsNullOrEmpty(methodName)) logger.LogInformation("Saved data from {methodName} successfully", methodName);
                return true;
            }

            if(methodName != null && !string.IsNullOrEmpty(methodName)) logger.LogInformation("Could not save data from {methodName}", methodName);
            return false;
        }
    }
}