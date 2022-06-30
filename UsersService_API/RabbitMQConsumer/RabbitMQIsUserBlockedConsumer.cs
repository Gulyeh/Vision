using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Messages;
using UsersService_API.RabbitMQSender;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQIsUserBlockedConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQIsUserBlockedConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQIsUserBlockedConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQIsUserBlockedConsumer> logger,
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
            channel.QueueDeclare(queue: "IsUserBlockedQueue", false, false, false, arguments: null);

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
                logger.LogInformation("Received message from queue: IsUserBlockedQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;
                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    ChatUsersDto? data = JsonConvert.DeserializeObject<ChatUsersDto>(content);
                    var result = HandleMessage(data).GetAwaiter().GetResult();
                    rabbitMQSender.SendMessage(result, args.BasicProperties.ReplyTo, prop);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                    rabbitMQSender.SendMessage(null, args.BasicProperties.ReplyTo, prop);
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("IsUserBlockedQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> HandleMessage(ChatUsersDto? data)
        {
            if (data is null) return true;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                return await userRepository.IsUserBlocked(data.SenderId, data.ReceiverId);
            }
            catch (Exception)
            {
                return true;
            }
        }
    }
}