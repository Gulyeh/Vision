using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Messages;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQMessageNotificationConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQMessageNotificationConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQMessageNotificationConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQMessageNotificationConsumer> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "UserMessageNotificationQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    logger.LogInformation("Received message from queue: UserMessageNotificationQueue");
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    ChatUsersDto? data = JsonConvert.DeserializeObject<ChatUsersDto>(content);
                    HandleMessage(data).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("UserMessageNotificationQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(ChatUsersDto? data)
        {
            if (data is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                await userRepository.SendUserMessageNotification(data.ReceiverId, data.SenderId);
            }
        }
    }
}