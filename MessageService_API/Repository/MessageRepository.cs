using AutoMapper;
using MessageService_API.DbContexts;
using MessageService_API.Dtos;
using MessageService_API.Entites;
using MessageService_API.Messages;
using MessageService_API.RabbitMQRPC;
using MessageService_API.RabbitMQSender;
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
        private readonly ILogger<MessageRepository> logger;
        private readonly IRabbitMQRPC rabbitMQRPC;
        private readonly IRabbitMQSender rabbitMQSender;

        public MessageRepository(IMapper mapper, ApplicationDbContext db, IUploadService uploadService,
            ILogger<MessageRepository> logger, IRabbitMQRPC rabbitMQRPC, IRabbitMQSender rabbitMQSender)
        {
            this.uploadService = uploadService;
            this.logger = logger;
            this.rabbitMQRPC = rabbitMQRPC;
            this.rabbitMQSender = rabbitMQSender;
            this.mapper = mapper;
            this.db = db;
        }

        public Task SendUserMessageNotification(Guid receiverId, Guid senderId)
        {
            rabbitMQSender.SendMessage(new ChatUsers(senderId, receiverId), "UserMessageNotificationQueue");
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteMessage(DeleteMessageDto message, bool IsDelete = false)
        {
            var findMessage = await FindMessage(message.ChatId, message.MessageId);
            if (findMessage is null) return false;

            if (findMessage.SenderId == message.userId) findMessage.SenderDeleted = true;
            else findMessage.ReceiverDeleted = true;

            if (!await SaveChangesAsync()) return false;

            if ((findMessage.SenderDeleted && findMessage.ReceiverDeleted) || IsDelete)
            {
                var attachments = findMessage.Attachments;
                db.Messages.Remove(findMessage);
                if (!await SaveChangesAsync()) return false;

                if (attachments is not null && attachments.Any())
                {
                    foreach (var attachment in attachments) await uploadService.DeletePhoto(attachment.AttachmentId);
                }
            }
            return true;
        }

        public async Task<(bool, bool)> EditMessage(EditMessageDto message)
        {
            var findMessage = await FindMessage(message.ChatId, message.MessageId);
            if (findMessage is null) return (false, false);
            if (findMessage.SenderId != message.SenderId) return (false, false);

            if (string.IsNullOrWhiteSpace(message.Content) && message.DeletedAttachmentsId.Count() == findMessage.Attachments.Count()) return (false, true);

            findMessage.Content = message.Content;
            findMessage.IsEdited = true;
            await SaveChangesAsync();

            if (message.DeletedAttachmentsId is not null && message.DeletedAttachmentsId.Any())
            {
                foreach (var id in message.DeletedAttachmentsId)
                {
                    var attachment = findMessage.Attachments.FirstOrDefault(x => x.Id == id);
                    if (attachment is not null)
                    {
                        db.MessagesAttachments.Remove(attachment);
                        if (await SaveChangesAsync()) await uploadService.DeletePhoto(attachment.AttachmentId);
                    }
                }
            }
            return (true, false);
        }

        public async Task<(IEnumerable<MessageDto>, int)> GetMessages(Guid chatId, Guid userId, int pageNumber = 1)
        {
            var findChat = await db.Chats.Include(x => x.Messages).ThenInclude(x => x.Attachments).AsSplitQuery().FirstOrDefaultAsync(x => x.Id == chatId);
            if (findChat is null) return (new List<MessageDto>(), 0);

            foreach (var message in findChat.Messages.Where(x => x.SenderId != userId && x.ChatId == chatId && x.DateRead is null)) message.DateRead = DateTime.Now;
            await db.SaveChangesAsync();

            var messages = findChat.Messages.AsQueryable().Where(x => (x.SenderId == userId && !x.SenderDeleted)
                || (x.ReceiverId == userId && !x.ReceiverDeleted));

            var paginatedMessages = messages.SkipLast((pageNumber - 1) * 25).TakeLast(25).OrderBy(x => x.MessageSent);

            var maxPages = (int)Math.Ceiling(decimal.Parse((messages.Count() / 25).ToString()));

            return (mapper.Map<IEnumerable<MessageDto>>(paginatedMessages), maxPages);
        }

        public async Task<(MessageDto?, bool)> SendMessage(AddMessageDto message)
        {
            var findChat = await db.Chats.FirstOrDefaultAsync(x => x.Id == message.ChatId);
            if (findChat is null) return (null, false);

            var response = await rabbitMQRPC.SendAsync("IsUserBlockedQueue", new ChatUsers(message.SenderId, message.ReceiverId));
            if (response is null || string.IsNullOrWhiteSpace(response) || bool.Parse(response)) return (null, true);

            var mapped = mapper.Map<Message>(message);

            if (message.AttachmentsList is not null && message.AttachmentsList.Any())
            {
                foreach (var attachment in message.AttachmentsList)
                {
                    var results = await uploadService.UploadPhoto(attachment);
                    if (results.Error is null)
                    {
                        var msgAttachment = new MessageAttachment()
                        {
                            AttachmentId = results.PublicId,
                            AttachmentUrl = results.SecureUrl.AbsoluteUri
                        };
                        mapped.Attachments.Add(msgAttachment);
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
                return (null, false);
            }

            return (mapper.Map<MessageDto>(mapped), false);
        }

        public async Task<MessageDto> GetMessage(Guid chatId, Guid messageId)
        {
            var findMessage = await FindMessage(chatId, messageId);
            return mapper.Map<MessageDto>(findMessage);
        }

        private async Task<Message?> FindMessage(Guid chatId, Guid messageId)
        {
            var findChat = await db.Chats
               .Include(x => x.Messages)
               .ThenInclude(x => x.Attachments)
               .AsSplitQuery()
               .FirstOrDefaultAsync(x => x.Id == chatId);
            if (findChat is null) return null;

            var findMessage = findChat.Messages.FirstOrDefault(x => x.Id == messageId);
            if (findMessage is null) return null;

            return findMessage;
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }

        public Task<ICollection<HasUnreadMessagesDto>> CheckUnreadMessages(ICollection<Guid> FriendsList, Guid UserId)
        {
            ICollection<HasUnreadMessagesDto> data = new List<HasUnreadMessagesDto>();
            foreach (var userId in FriendsList)
            {
                var hasUnreadMessages = db.Messages.Any(x => x.SenderId == userId && x.ReceiverId == UserId && x.DateRead == null);
                data.Add(new HasUnreadMessagesDto(userId, hasUnreadMessages));
            }
            return Task.FromResult(data);
        }
    }
}