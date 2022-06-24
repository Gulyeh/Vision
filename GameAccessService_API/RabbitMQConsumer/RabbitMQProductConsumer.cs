using GameAccessService_API.Helpers;
using GameAccessService_API.Messages;
using GameAccessService_API.RabbitMQSender;
using GameAccessService_API.Repository.IRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace GameAccessService_API.RabbitMQConsumer
{
    public class RabbitMQProductConsumer  : BackgroundService
    {
        private readonly ILogger<RabbitMQProductConsumer> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQProductConsumer (IOptions<RabbitMQSettings> options, ILogger<RabbitMQProductConsumer> logger, IServiceScopeFactory scopeFactory, IRabbitMQSender rabbitMQSender)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
            this.rabbitMQSender = rabbitMQSender;

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "DeleteProductAccessQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("RabbitMQ Consumed request from queue: DeleteProductAccessQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                Guid? productId = JsonConvert.DeserializeObject<Guid>(content);
                HandleMessage(productId).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("DeleteProductAccessQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(Guid? productId)
        {
            if (productId is not null && productId != Guid.Empty)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var accessRepository = scope.ServiceProvider.GetRequiredService<IAccessRepository>();
                    await accessRepository.RemoveProductAccess((Guid)productId);
                    scope.Dispose();
                }
                catch (Exception)
                { }
            }
        }
    }
}