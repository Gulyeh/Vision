using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VisionClient.Core;
using VisionClient.Core.Models;

namespace VisionClient.SignalR
{
    internal interface IMessageService_Hubs
    {
        HubConnection? MessageHubConnection { get; set; }
        Task Send(string methodName, object? data);
        Task CreateHubConnection(Guid receiverId);
        Task Disconnect();
    }

    internal class MessageService_Hubs : IMessageService_Hubs
    {
        public HubConnection? MessageHubConnection { get; set; }
        Guid ReceiverId;
        private UserModel? User;
        private readonly IStaticData StaticData;

        public MessageService_Hubs(IStaticData staticData)
        {
            this.StaticData = staticData;
        }

        public async Task CreateHubConnection(Guid receiverId)
        {
            if (receiverId != Guid.Empty)
            {
                ReceiverId = receiverId;
                User = StaticData.FriendsList.FirstOrDefault(x => x.UserId == ReceiverId);
            }

            MessageHubConnection = new HubConnectionBuilder()
                .WithUrl(ConnectionData.MessageHub, opts =>
                {
                    opts.AccessTokenProvider = () => Task.FromResult(StaticData.UserData?.Access_Token);
                    opts.Headers.Add("ReceiverId", ReceiverId.ToString());
                })
                .WithAutomaticReconnect()
                .Build();

            ListenMessageConnections();
            ListenSelfConnections();
            await MessageHubConnection.StartAsync();
        }

        private void ListenSelfConnections()
        {
            if (MessageHubConnection is null) return;
            MessageHubConnection.On<ObservableCollection<MessageModel>, Guid, int>("GetMessages", (messages, chatId, maxPages) =>
            {
                if (User is null || messages is null) return;
                foreach (var message in messages.Where(x => x.SenderId == ReceiverId)) message.User = User;

                if (StaticData.Messages.Count > 0) foreach (var message in messages) StaticData.Messages.Insert(0, message);
                else StaticData.Messages.AddRange(messages);

                StaticData.ChatId = chatId;
                StaticData.MaxPages = maxPages;
                User.HasUnreadMessages = false;
            });

            MessageHubConnection.On<Guid>("RemovedMessage", (id) =>
            {
                if (id == Guid.Empty) return;
                var message = StaticData.Messages.FirstOrDefault(x => x.Id == id);
                if (message is not null)
                {
                    App.Current.Dispatcher.Invoke(delegate
                    {
                        StaticData.Messages.Remove(message);
                    });
                }
            });
        }

        private void ListenMessageConnections()
        {
            if (MessageHubConnection is null) return;

            MessageHubConnection.On<MessageModel>("NewMessage", (message) =>
            {
                if (User is null) return;
                if (message.SenderId != StaticData.UserData.UserId) message.User = User;

                App.Current.Dispatcher.Invoke(delegate
                {
                    StaticData.Messages.Add(message);
                });
            });

            MessageHubConnection.On<MessageModel>("MessageEdited", (message) =>
            {
                if (User is null || message is null) return;
                if (message.SenderId != StaticData.UserData.UserId) message.User = User;
                var messageFound = StaticData.Messages.FirstOrDefault(x => x.Id == message.Id);
                if (messageFound is not null)
                {
                    messageFound.Content = message.Content;
                    messageFound.Attachments = message.Attachments;
                    messageFound.IsEdited = message.IsEdited;
                }
            });

            MessageHubConnection.On<DateTime>("ChatUserConnected", (date) =>
            {
                var messages = StaticData.Messages.Where(x => x.Id != StaticData.UserData.UserId && x.DateRead is null);
                foreach (var message in messages) message.DateRead = date;
            });
        }

        public async Task Disconnect()
        {
            if (MessageHubConnection is null) return;
            await MessageHubConnection.DisposeAsync();
            MessageHubConnection = null;
        }

        public async Task Send(string methodName, object? data)
        {
            if (MessageHubConnection is not null) await MessageHubConnection.SendAsync(methodName, data);
        }
    }
}
