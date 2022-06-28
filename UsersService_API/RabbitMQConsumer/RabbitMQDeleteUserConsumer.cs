using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQDeleteUserConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQDeleteUserConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQDeleteUserConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQDeleteUserConsumer> logger)
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
            channel.QueueDeclare(queue: "DeleteAccountQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: DeleteAccountQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                Guid? userId = JsonConvert.DeserializeObject<Guid>(content);
                HandleMessage(userId).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("DeleteAccountQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(Guid? data)
        {
            if (data is not null && data != Guid.Empty)
            {
                try{
                    using var scope = serviceScopeFactory.CreateScope();
                    var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                    await userRepository.DeleteUser((Guid)data);
                }catch(Exception){}
            }
        }
    }
}