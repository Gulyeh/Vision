using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageService_API.Helpers;
using MessageService_API.Messages;
using MessageService_API.Repository.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MessageService_API.RabbitMQConsumer
{
    public class RabbitMQUsersConsumer : BackgroundService
    {
        private readonly IChatRepository chatRepository;
        private IConnection connection;
        private IModel channel;

        public RabbitMQUsersConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "DeleteChatQueue", false, false, false, arguments: null);
            
            using var scope = scopeFactory.CreateScope();
            chatRepository = scope.ServiceProvider.GetRequiredService<IChatRepository>();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                DeleteChat? chatData = JsonConvert.DeserializeObject<DeleteChat>(content);
                HandleMessage(chatData).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("DeleteChatQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(DeleteChat? data)
        {
            if(data is not null){ 
                await chatRepository.DeleteChat(data.User1, data.User2);
            }
        }
    }
}