using MessageService_API.Dtos;
using MessageService_API.Helpers;
using MessageService_API.RabbitMQSender;
using MessageService_API.Repository.IRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MessageService_API.RabbitMQConsumer
{
    public class RabbitMQUnreadMessagesConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQUnreadMessagesConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQUnreadMessagesConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQUnreadMessagesConsumer> logger,
            IRabbitMQSender rabbitMQSender)
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
            channel.QueueDeclare(queue: "CheckUnreadMessagesQueue", false, false, false, arguments: null);

            this.scopeFactory = scopeFactory;
            this.logger = logger;
            this.rabbitMQSender = rabbitMQSender;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: CheckUnreadMessagesQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;

                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    ICollection<Guid>? friendsList = JsonConvert.DeserializeObject<ICollection<Guid>>(content);

                    args.BasicProperties.Headers.TryGetValue("userId", out object? userIdBytes);
                    Guid parsedUserId = Guid.Empty;
                    if (userIdBytes is not null)
                    {
                        var userId = Encoding.UTF8.GetString((byte[])userIdBytes);
                        Guid.TryParse(userId, out parsedUserId);
                    }
                    var result = HandleMessage(friendsList, parsedUserId).GetAwaiter().GetResult();

                    rabbitMQSender.SendMessage(result, args.BasicProperties.ReplyTo, prop);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    rabbitMQSender.SendMessage(null, args.BasicProperties.ReplyTo, prop);
                }

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CheckUnreadMessagesQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<object> HandleMessage(ICollection<Guid>? friendsList, Guid userId)
        {
            if (friendsList is null || userId == Guid.Empty) return new List<HasUnreadMessagesDto>();
            try
            {
                using var scope = scopeFactory.CreateScope();
                var messageRepository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                return await messageRepository.CheckUnreadMessages(friendsList, userId);
            }
            catch (Exception)
            {
                return new List<HasUnreadMessagesDto>();
            }
        }
    }
}