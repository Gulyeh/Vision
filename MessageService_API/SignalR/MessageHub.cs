using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MessageService_API.Dtos;
using MessageService_API.Repository.IRepository;
using MessageService_API.Services.IServices;
using MessagesService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;

namespace MessageService_API.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageRepository messageRepository;
        private readonly IChatRepository chatRepository;
        private readonly Guid userId;
        private readonly IConnectionsCacheService cacheService;
        private readonly IUsersService usersService;

        public MessageHub(IMessageRepository messageRepository, IChatRepository chatRepository, IConnectionsCacheService cacheService, IUsersService usersService)
        {
            this.cacheService = cacheService;
            this.usersService = usersService;
            this.messageRepository = messageRepository;
            this.chatRepository = chatRepository;
            userId = Context.User != null ? Context.User.GetId() : Guid.Empty;
        }

        public override async Task OnConnectedAsync()
        {
                var token = Context.GetHttpContext()?.Request.Query["access_token"];
                var receiverId = Context.GetHttpContext()?.Request.Query["receiverId"];
                Guid chatGuid = Guid.Empty;
                Guid revceiverGuid = Guid.Empty;

                Guid.TryParse(receiverId, out revceiverGuid);
                if(revceiverGuid == Guid.Empty || string.IsNullOrEmpty(token)) throw new HubException("Wrong data provided");
                var chat = await chatRepository.ChatExists(revceiverGuid, userId);
      
                if(chat == Guid.Empty){
                    var guid = await chatRepository.CreateChat(revceiverGuid, userId, token);
                    if(guid == Guid.Empty) throw new HubException("Something went wrong");
                    chatGuid = guid;
                }
                else chatGuid = chat;

                await Groups.AddToGroupAsync(Context.ConnectionId, chatGuid.ToString());
                await cacheService.AddToGroupCache(chatGuid, Context.ConnectionId);
                var messages = await messageRepository.GetMessages(chatGuid, userId);
                await Clients.Caller.SendAsync("GetMessages", messages);
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var receiverId = Context.GetHttpContext()?.Request.Query["receiverId"];
            Guid revceiverGuid = Guid.Empty;
            Guid.TryParse(receiverId, out revceiverGuid);
            if(revceiverGuid == Guid.Empty) throw new HubException("Please provide a valid receiverId");

            var chat = await chatRepository.ChatExists(revceiverGuid, userId);
            if(chat == Guid.Empty) throw new HubException("This chat does not exist");
   
            await cacheService.RemoveFromGroupCache(chat, userId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.ToString());
        }
        
        public async Task SendMessage(AddMessageDto data){
            data.SenderId = userId;
            var connIds = await cacheService.GetFromGroupCache(data.ChatId);
            var isOtherMember = connIds.Any(x => x.Key != userId.ToString());
            if(isOtherMember){
                data.DateRead = DateTime.UtcNow;
            }
            var message = await messageRepository.SendMessage(data);

            if(!isOtherMember){
                var access_token = Context.GetHttpContext()?.Request.Query["access_token"];
                if(!string.IsNullOrEmpty(access_token)) await messageRepository.SendUserMessageNotification(data.ReceiverId, data.ChatId, access_token);
            }

            await Clients.Group(data.ChatId.ToString()).SendAsync("NewMessage", message);
        }

        public async Task RemoveMessage(DeleteMessageDto message){
            message.userId = userId;
            if(await messageRepository.DeleteMessage(message)) await Clients.Caller.SendAsync("RemovedMessage", message.MessageId);
        }

        public async Task EditMessage(EditMessageDto message){
            if(await messageRepository.EditMessage(message)){
                var messageDto = await messageRepository.GetMessage(message.ChatId, message.MessageId);
                if(messageDto.Id != Guid.Empty) await Clients.Group(message.ChatId.ToString()).SendAsync("MessageEdited", messageDto);
            }
        }

    }
}