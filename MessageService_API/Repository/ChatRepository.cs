using MessageService_API.Builders;
using MessageService_API.DbContexts;
using MessageService_API.Entites;
using MessageService_API.Repository.IRepository;
using MessageService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace MessageService_API.Repository
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IUsersService usersService;
        private readonly ILogger<ChatRepository> logger;
        private readonly IChatCacheService chatCacheService;
        private readonly IUploadService uploadService;

        public ChatRepository(ApplicationDbContext db, IUsersService usersService, ILogger<ChatRepository> logger, 
            IChatCacheService chatCacheService, IUploadService uploadService)
        {
            this.db = db;
            this.usersService = usersService;
            this.logger = logger;
            this.chatCacheService = chatCacheService;
            this.uploadService = uploadService;
        }

        public async Task DeleteChat(Guid user1, Guid user2)
        {
            Chat? chat;
            var cachedId = await chatCacheService.GetFromChatCache(user1, user2);
            if (cachedId != Guid.Empty) chat = await db.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == cachedId);
            else chat = await db.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => (x.User1 == user1 && x.User2 == user2)
                        || (x.User1 == user2 && x.User2 == user1));
            

            if (chat is not null)
            {
                var messages = await chat.Messages.AsQueryable().Include(x => x.Attachments).ToListAsync();
                db.Chats.Remove(chat);
                if (await SaveChangesAsync())
                {
                    await chatCacheService.RemoveFromChatCache(user1, user2);
                    await RemoveAttachmentsFromCloud(messages);
                    logger.LogInformation("Deleted chat between User with ID: {Id1} and User with ID: {Id2}", user1, user2); 
                }else{
                    logger.LogError("Could not delete chat between User with ID: {Id1} and User with ID: {Id2}", user1, user2); 
                }
            }
        }

        public async Task<Guid> ChatExists(Guid user1, Guid user2)
        {
            var cachedId = await chatCacheService.GetFromChatCache(user1, user2);
            if (cachedId != Guid.Empty) return cachedId;

            var chat = await db.Chats.FirstOrDefaultAsync(x => (x.User1 == user1 && x.User2 == user2)
                || (x.User1 == user2 && x.User2 == user1));

            if (chat is null) return Guid.Empty;

            await chatCacheService.AddToChatCache(chat.Id, user1, user2);
            return chat.Id;
        }

        public async Task<Guid> CreateChat(Guid receiverId, Guid senderId, string Access_Token)
        {
            if (!await usersService.CheckUserExists<bool>(receiverId, Access_Token)) return Guid.Empty;

            var chatBuilder = new ChatBuilder();
            chatBuilder.SetUser1(receiverId);
            chatBuilder.SetUser2(senderId);
            var chat = chatBuilder.Build();

            await db.Chats.AddAsync(chat);
            if (await SaveChangesAsync())
            {
                await chatCacheService.AddToChatCache(chat.Id, receiverId, senderId);
                logger.LogInformation("Created chat between User with ID: {Id1} and User with ID: {Id2}", receiverId, senderId); 
                return chat.Id;
            }
            
            logger.LogError("Could not create chat between User with ID: {Id1} and User with ID: {Id2}", receiverId, senderId); 
            return Guid.Empty;
        }

        private async Task RemoveAttachmentsFromCloud(List<Message> messages)
        {
            foreach (var message in messages)
            {
                if (message.Attachments is not null)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        await uploadService.DeletePhoto(attachment.AttachmentId);
                    }
                }
            }
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}