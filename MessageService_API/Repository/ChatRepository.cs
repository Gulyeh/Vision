using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IChatCacheService chatCacheService;
        private readonly IUploadService uploadService;

        public ChatRepository(ApplicationDbContext db, IUsersService usersService, IChatCacheService chatCacheService, IUploadService uploadService)
        {
            this.db = db;
            this.usersService = usersService;
            this.chatCacheService = chatCacheService;
            this.uploadService = uploadService;
        }

        public async Task DeleteChat(Guid user1, Guid user2){
            Chat? chat;
            var cachedId = await chatCacheService.GetFromChatCache(user1, user2);
            if(cachedId != Guid.Empty){
                chat = await db.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => x.Id == cachedId);
            }else{
                chat = await db.Chats.Include(x => x.Messages).FirstOrDefaultAsync(x => (x.User1 == user1 && x.User2 == user2) 
                || (x.User1 == user2 && x.User2 == user1));
            }

            if(chat is not null){
                var messages = await chat.Messages.AsQueryable().Include(x => x.Attachments).ToListAsync();     
                db.Chats.Remove(chat);
                if(await SaveChangesAsync()){
                    await chatCacheService.RemoveFromChatCache(user1, user2);
                    await RemoveAttachmentsFromCloud(messages);
                }
            }
        }

        public async Task<Guid> ChatExists(Guid user1, Guid user2){
            var cachedId = await chatCacheService.GetFromChatCache(user1, user2);
            if(cachedId != Guid.Empty) return cachedId;

            var chat = await db.Chats.FirstOrDefaultAsync(x => (x.User1 == user1 && x.User2 == user2) 
                || (x.User1 == user2 && x.User2 == user1));

            if(chat is null) return Guid.Empty;

            await chatCacheService.AddToChatCache(chat.Id, user1, user2);
            return chat.Id;
        }

        public async Task<Guid> CreateChat(Guid receiverId, Guid senderId, string Access_Token)
        {
            if(!await usersService.CheckUserExists<bool>(receiverId, Access_Token)) return Guid.Empty;
            
            var chat = new Chat(){
                User1 = receiverId,
                User2 = senderId
            };

            await db.Chats.AddAsync(chat);
            if(await SaveChangesAsync()){
                await chatCacheService.AddToChatCache(chat.Id, receiverId, senderId);
                return chat.Id;
            } 
            return Guid.Empty;
        }

        private async Task RemoveAttachmentsFromCloud(List<Message> messages){
            foreach(var message in messages){
                if(message.Attachments is not null){
                    foreach(var attachment in message.Attachments){
                        await uploadService.DeletePhoto(attachment.AttachmentId);
                    }
                }
            }
        }

        private async Task<bool> SaveChangesAsync(){
            if(await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}