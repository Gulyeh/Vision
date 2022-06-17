using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQIdentityConsumer : BackgroundService
    {
        private class Message
        {
            public string? userId { get; set; }
        }

        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQIdentityConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQIdentityConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQIdentityConsumer> logger)
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
            channel.QueueDeclare(queue: "CreateUserQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: CreateUserQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                Message? userId = JsonConvert.DeserializeObject<Message>(content);
                HandleMessage(userId).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CreateUserQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(Message? data)
        {
            if (data is not null)
            {
                Guid Id;
                Guid.TryParse(data.userId, out Id);
                if (Id != Guid.Empty)
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    await userRepository.CreateUser(Id);
                }
            }
        }
    }
}