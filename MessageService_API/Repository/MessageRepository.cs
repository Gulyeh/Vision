using AutoMapper;
using MessageService_API.DbContexts;
using MessageService_API.Dtos;
using MessageService_API.Entites;
using MessageService_API.Repository.IRepository;
using MessageService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace MessageService_API.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;
        private readonly IUploadService uploadService;
        private readonly IUsersService usersService;
        private readonly ILogger<MessageRepository> logger;

        public MessageRepository(IMapper mapper, ApplicationDbContext db, IUploadService uploadService, 
            IUsersService usersService, ILogger<MessageRepository> logger)
        {
            this.uploadService = uploadService;
            this.usersService = usersService;
            this.logger = logger;
            this.mapper = mapper;
            this.db = db;
        }

        public async Task SendUserMessageNotification(Guid chatId, Guid receiverId, string Access_Token)
        {
            await usersService.SendUserMessageNotification<bool>(receiverId, chatId, Access_Token);
        }

        public async Task<bool> DeleteMessage(DeleteMessageDto message)
        {
            var findMessage = await FindMessage(message.ChatId, message.MessageId);
            if(findMessage is null) return false;

            if (findMessage.SenderId == message.userId) findMessage.SenderDeleted = true;
            else findMessage.ReceiverDeleted = true;

            if (!await SaveChangesAsync()) return false;

            if (findMessage.SenderDeleted && findMessage.ReceiverDeleted)
            {
                var attachments = findMessage.Attachments;
                db.Messages.Remove(findMessage);
                if (!await SaveChangesAsync()) return false;

                if (attachments is not null && attachments.Any())
                {
                    foreach (var attachment in attachments)
                    {
                        await uploadService.DeletePhoto(attachment.AttachmentId);
                    }
                }
            }
            return true;
        }

        public async Task<bool> EditMessage(EditMessageDto message)
        {
            var findMessage = await FindMessage(message.ChatId, message.MessageId);
            if(findMessage is null) return false;

            findMessage.Content = message.Content;
            findMessage.IsEdited = true;
            if (!await SaveChangesAsync()) {
                logger.LogError("Could not edit message with ID: {messageId} in Chat with ID: {chatId}", message.MessageId, message.ChatId); 
                return false;
            }

            if (message.DeletedAttachmentsId is not null && message.DeletedAttachmentsId.Any())
            {
                foreach (var id in message.DeletedAttachmentsId)
                {
                    var attachment = findMessage.Attachments?.FirstOrDefault(x => x.Id == id);
                    if (attachment is not null)
                    {
                        db.MessagesAttachments.Remove(attachment);
                        if (await SaveChangesAsync()) await uploadService.DeletePhoto(attachment.AttachmentId);
                    }
                }
            }

            return true;
        }

        public async Task<IEnumerable<MessageDto>> GetMessages(Guid chatId, Guid userId)
        {
            var findChat = await db.Chats.Include(x => x.Messages)
                .FirstOrDefaultAsync(x => x.Id == chatId);
            if (findChat is null) return new List<MessageDto>();

            var messages = findChat.Messages.AsQueryable().Include(x => x.Attachments).Where(x => (x.SenderId == userId && !x.SenderDeleted)
                || (x.ReceiverId == userId && !x.ReceiverDeleted));

            return mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<MessageDto> SendMessage(AddMessageDto message)
        {
            var findChat = await db.Chats.FirstOrDefaultAsync(x => x.Id == message.ChatId);
            if (findChat is null) return new MessageDto();
            
            var mapped = mapper.Map<Message>(message);

            if (message.Attachments is not null && message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                {
                    var results = await uploadService.UploadPhoto(attachment);
                    if (results.Error is null)
                    {
                        var msgAttachment = new MessageAttachment()
                        {
                            AttachmentId = results.PublicId,
                            AttachmentUrl = results.SecureUrl.AbsoluteUri
                        };
                        mapped.Attachments?.Add(msgAttachment);
                    }
                }
            }

            findChat.Messages.Add(mapped);

            if (!await SaveChangesAsync())
            {
                if (mapped.Attachments is not null && mapped.Attachments.Any())
                {
                    foreach (var attachment in mapped.Attachments)
                    {
                        await uploadService.DeletePhoto(attachment.AttachmentId);
                    }
                }
                logger.LogError("Could not send message to Chat with ID: {chatId}", message.ChatId); 
                return new MessageDto();
            }

            return mapper.Map<MessageDto>(mapped);
        }

        public async Task<MessageDto> GetMessage(Guid chatId, Guid messageId)
        {
            var findMessage = await FindMessage(chatId, messageId);
            return mapper.Map<MessageDto>(findMessage);
        }

        private async Task<Message?> FindMessage(Guid chatId, Guid messageId){
            var findChat = await db.Chats
               .Include(x => x.Messages)
               .FirstOrDefaultAsync(x => x.Id == chatId);
            if (findChat is null) return null;

            var findMessage = findChat.Messages.AsQueryable().Include(x => x.Attachments).FirstOrDefault(x => x.Id == messageId);
            if (findMessage is null) return null;

            return findMessage;
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}