using MessageService_API.Dtos;
using MessageService_API.Messages;
using MessageService_API.Repository.IRepository;
using MessageService_API.Services.IServices;
using MessagesService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace MessageService_API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IChatRepository chatRepository;
        private readonly IConnectionsCacheService cacheService;
        private readonly IUsersService usersService;
        private readonly ILogger<MessageHub> logger;
        private readonly IChatCacheService chatCacheService;

        public MessageHub(IMessageRepository messageRepository, IChatRepository chatRepository,
            IConnectionsCacheService cacheService, IUsersService usersService, ILogger<MessageHub> logger, IChatCacheService chatCacheService)
        {
            this.cacheService = cacheService;
            this.usersService = usersService;
            this.logger = logger;
            this.chatCacheService = chatCacheService;
            this.messageRepository = messageRepository;
            this.chatRepository = chatRepository;
        }

        private Guid GetId()
        {
            return Context.User != null ? Context.User.GetId() : Guid.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetId();
            var token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
            var receiverId = Context.GetHttpContext()?.Request.Headers["ReceiverId"];

            Guid.TryParse(receiverId, out Guid receiverGuid);
            if (receiverGuid == Guid.Empty || string.IsNullOrEmpty(token)) return;
            Guid chat = await chatRepository.ChatExists(receiverGuid, userId);

            if (chat == Guid.Empty)
            {
                var guid = await chatRepository.CreateChat(receiverGuid, userId, token);
                if (guid == Guid.Empty) return;
                chat = guid;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, chat.ToString());
            await cacheService.AddToGroupCache(chat, userId, Context.ConnectionId);
            (var messages, int maxPages) = await messageRepository.GetMessages(chat, userId);
            await Clients.Caller.SendAsync("GetMessages", messages, chat, maxPages);

            var connIds = await cacheService.GetFromGroupCache(chat);
            var otherMemberIds = connIds.GetValueOrDefault(receiverGuid);
            if (otherMemberIds is not null)
            {
                if (otherMemberIds.Count() > 0) await Clients.Clients(otherMemberIds).SendAsync("ChatUserConnected", DateTime.Now);
            }

            logger.LogInformation("User with ConnectionId: {connId} has connected to MessageHub", Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var receiverId = Context.GetHttpContext()?.Request.Headers["ReceiverId"];
            var userId = GetId();

            Guid.TryParse(receiverId, out Guid revceiverGuid);
            if (revceiverGuid == Guid.Empty) return;

            var chat = await chatRepository.ChatExists(revceiverGuid, userId);
            if (chat == Guid.Empty) return;

            await cacheService.RemoveFromGroupCache(chat, userId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.ToString());

            logger.LogInformation("User with ConnectionId: {connId} has disconnected from MessageHub", Context.ConnectionId);
        }

        public async Task SendMessage(AddMessageDto data)
        {
            if (string.IsNullOrWhiteSpace(data.Content) && data.AttachmentsList.Count() == 0) return;

            var userId = GetId();
            var receiverId = Context.GetHttpContext()?.Request.Headers["ReceiverId"];
            var token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
            Guid.TryParse(receiverId, out Guid ReceiverId);

            if (string.IsNullOrEmpty(token) || ReceiverId == Guid.Empty)
            {
                await SendSystemMessage("Cannot send message");
                return;
            }

            data.SenderId = userId;
            data.ReceiverId = ReceiverId;

            var chatExists = await chatCacheService.GetFromChatCache(data.ReceiverId, data.SenderId);
            if (chatExists == Guid.Empty)
            {
                await SendSystemMessage("Cannot send message, chat does not exist");
                return;
            }

            var connIds = await cacheService.GetFromGroupCache(data.ChatId);
            var isOtherMember = connIds.Any(x => x.Key != userId);
            if (isOtherMember) data.DateRead = DateTime.UtcNow;

            (var message, bool isBlocked) = await messageRepository.SendMessage(data, token);
            if (message is null && !isBlocked)
            {
                await SendSystemMessage("Cannot send message");
                return;
            }

            if (isBlocked)
            {
                await SendSystemMessage("Cannot send message. One of the users is blocked");
                return;
            }

            if (!isOtherMember)
            {
                var access_token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
                if (!string.IsNullOrEmpty(access_token)) await messageRepository.SendUserMessageNotification(data.ReceiverId, userId, access_token);
            }

            await Clients.Group(data.ChatId.ToString()).SendAsync("NewMessage", message);
        }

        public async Task RemoveMessage(DeleteMessageDto message)
        {
            message.userId = GetId();
            if (await messageRepository.DeleteMessage(message)) await CheckMutlipleClients(message.userId, message.ChatId, "RemovedMessage", message.MessageId);
        }

        public async Task EditMessage(EditMessageDto message)
        {
            message.SenderId = GetId();
            (bool IsDeleted, bool ToDelete) = await messageRepository.EditMessage(message);
            if (IsDeleted)
            {
                var messageDto = await messageRepository.GetMessage(message.ChatId, message.MessageId);
                if (messageDto.Id != Guid.Empty) await Clients.Group(message.ChatId.ToString()).SendAsync("MessageEdited", messageDto);
                return;
            }

            if (ToDelete)
            {
                if (await messageRepository.DeleteMessage(new DeleteMessageDto() { MessageId = message.MessageId, ChatId = message.ChatId }, true))
                {
                    await Clients.Group(message.ChatId.ToString()).SendAsync("RemovedMessage", message.MessageId);
                }
            }
        }

        public async Task GetMoreMessages(GetMoreMessagesData data)
        {
            var userId = GetId();
            if (data.ChatId == Guid.Empty || data.PageNumber < 1 || userId == Guid.Empty) return;
            (var messages, int maxPages) = await messageRepository.GetMessages(data.ChatId, userId, data.PageNumber);
            await Clients.Caller.SendAsync("GetMessages", messages, data.ChatId, maxPages);
        }

        private async Task SendSystemMessage(string content) => await Clients.Caller.SendAsync("NewMessage", new MessageDto() { SenderId = GetId(), Content = $"(System) {content}" });

        private async Task CheckMutlipleClients(Guid userId, Guid chatId, string connName, object? data)
        {
            var connIds = await cacheService.GetFromGroupCache(chatId);
            var userConnIds = connIds.GetValueOrDefault(userId);
            if (userConnIds is null || !userConnIds.Any()) return;
            await Clients.Clients(userConnIds).SendAsync(connName, data);
        }

    }
}