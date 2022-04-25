using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQIdentityConsumer : BackgroundService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<RabbitMQIdentityConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQIdentityConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQIdentityConsumer> logger)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "CreateUserQueue", false, false, false, arguments: null);
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
                HandleMessage(content).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CreateUserQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(string userId)
        {
            Guid Id;
            Guid.TryParse(userId, out Id);
            if (Id != Guid.Empty) await userRepository.CreateUser(Id);
        }
    }
}