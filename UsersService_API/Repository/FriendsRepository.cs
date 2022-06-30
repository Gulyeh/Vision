using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using UsersService_API.DbContexts;
using UsersService_API.Dtos;
using UsersService_API.Entites;
using UsersService_API.Messages;
using UsersService_API.RabbitMQRPC;
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
        private readonly IRabbitMQRPC rabbitMQRPC;

        public FriendsRepository(ApplicationDbContext db, IMapper mapper, IRabbitMQSender rabbitMQSender,
            ILogger<FriendsRepository> logger, IRabbitMQRPC rabbitMQRPC)
        {
            this.db = db;
            this.mapper = mapper;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.rabbitMQRPC = rabbitMQRPC;
        }

        public async Task<(bool, bool)> ToggleBlock(Guid userId, Guid UserToToggleId)
        {
            var blocked = await db.BlockedUsers.FirstOrDefaultAsync(x => x.BlockerId == userId && x.BlockedId == UserToToggleId);
            if (blocked is null)
            {
                var areFriends = db.UsersFriends.Any(x => (x.User1 == userId && x.User2 == UserToToggleId) || (x.User1 == UserToToggleId && x.User2 == userId));
                if (!areFriends) return (false, false);
                await db.BlockedUsers.AddAsync(new BlockedUsers(userId, UserToToggleId));
            }
            else db.BlockedUsers.Remove(blocked);
            return (await SaveChangesAsync(), blocked is null);
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

            return await SaveChangesAsync();
        }

        public async Task<bool> DeclineFriendRequest(Guid userId, Guid SenderId)
        {
            var findRequest = await db.FriendRequests.FirstOrDefaultAsync(x => (x.Sender == SenderId && x.Receiver == userId) || (x.Sender == userId && x.Receiver == SenderId));
            if (findRequest is null) return false;

            db.FriendRequests.Remove(findRequest);
            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteFriend(Guid userId, Guid ToDeleteUserId)
        {
            var findFriends = await db.UsersFriends.FirstOrDefaultAsync(x => (x.User1 == userId && x.User2 == ToDeleteUserId)
            || (x.User1 == ToDeleteUserId && x.User2 == userId));
            if (findFriends is null) return false;

            var blocked = await db.BlockedUsers.FirstOrDefaultAsync(x => (x.BlockedId == userId && x.BlockerId == ToDeleteUserId) || (x.BlockedId == ToDeleteUserId && x.BlockerId == userId));
            if (blocked is not null) db.BlockedUsers.Remove(blocked);

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

        public async Task<IEnumerable<GetFriendsDto>> GetFriends(Guid userId)
        {
            var findFriends = await db.UsersFriends.Where(x => x.User1 == userId || x.User2 == userId).ToListAsync();
            var blockedUsers = await db.BlockedUsers.Where(x => x.BlockerId == userId).ToListAsync();
            List<GetFriendsDto> friendList = new List<GetFriendsDto>();

            foreach (var friend in findFriends)
            {
                Guid User;
                _ = friend.User1 != userId ? User = friend.User1 : User = friend.User2;
                var data = await db.Users.FirstOrDefaultAsync(x => x.UserId == User);

                if (data is not null)
                {
                    var friendDto = mapper.Map<GetFriendsDto>(data);
                    friendDto.IsBlocked = blockedUsers.Any(x => x.BlockedId == User);
                    friendList.Add(friendDto);
                }
            }

            ICollection<Guid> friends = new List<Guid>(friendList.Select(x => x.UserId));
            var response = await rabbitMQRPC.SendAsync("CheckUnreadMessagesQueue", friends, userId);
            var json = JsonConvert.DeserializeObject<IEnumerable<HasUnreadMessagesDto>>(response);

            if (json is not null && json.Any())
            {
                foreach (var friend in json.Where(x => x.HasUnreadMessages == true))
                {
                    var user = friendList.FirstOrDefault(x => x.UserId == friend.UserId);
                    if (user is null) continue;
                    user.HasUnreadMessages = friend.HasUnreadMessages;
                }
            }
            return friendList;
        }

        public async Task<bool> SendFriendRequest(FriendRequestDto data)
        {
            var isFriend = db.UsersFriends.Any(x => (x.User1 == data.Sender && x.User2 == data.Receiver) || (x.User1 == data.Receiver && x.User2 == data.Sender));
            if (isFriend) return false;

            var requestExists = db.FriendRequests.Any(x => (x.Sender == data.Sender && x.Receiver == data.Receiver) || (x.Sender == data.Receiver && x.Receiver == data.Sender));
            if (requestExists) return false;

            var mapped = mapper.Map<FriendRequests>(data);
            await db.FriendRequests.AddAsync(mapped);

            return await SaveChangesAsync();
        }

        public async Task<IEnumerable<GetFriendRequestsDto>> GetFriendRequests(Guid userId)
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

            return requests;
        }

        public async Task<IEnumerable<GetPendingRequestsDto>> GetPendingRequests(Guid userId)
        {
            var findSentRequests = await db.FriendRequests.Where(x => x.Sender == userId).ToListAsync();
            List<GetPendingRequestsDto> requests = new List<GetPendingRequestsDto>();

            foreach (var data in findSentRequests)
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.UserId == data.Receiver);
                if (user is not null)
                {
                    var request = mapper.Map<GetPendingRequestsDto>(user);
                    requests.Add(request);
                }
            }

            return requests;
        }

        private async Task<bool> SaveChangesAsync()
        {
            return await db.SaveChangesAsync() > 0;
        }
    }
}