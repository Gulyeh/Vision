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
        private readonly Guid userId;
        private readonly ICacheService cacheService;

        public MessageHub(IMessageRepository messageRepository, ICacheService cacheService)
        {
            this.cacheService = cacheService;
            this.messageRepository = messageRepository;
            userId = Context.User != null ? Context.User.GetId() : Guid.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            var chatId = Context.GetHttpContext()?.Request.Query["chatId"].ToString();
            if(string.IsNullOrEmpty(chatId)) throw new HubException("Please provide a valid chatId");
            var chatGuid = Guid.Parse(chatId);

            await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
            await cacheService.AddToGroupCache(chatGuid, Context.ConnectionId);

            var messages = await messageRepository.GetMessages(chatGuid, userId);
            await Clients.Caller.SendAsync("GetMessages", messages);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var chatId = Context.GetHttpContext()?.Request.Query["chatId"].ToString();
            if(string.IsNullOrEmpty(chatId)) throw new HubException("Please provide a valid chatId");
            
            await cacheService.RemoveFromGroupCache(Guid.Parse(chatId), userId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
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
                //ask userservice if user is online and get his ID
                //if user is online, send him notification
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