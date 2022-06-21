using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProductsService_API.Helpers;
using ProductsService_API.Messages;
using ProductsService_API.Repository.IRepository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace ProductsService_API.RabbitMQConsumer
{
    public class RabbitMQEditGameConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQEditGameConsumer> logger;

        public RabbitMQEditGameConsumer(IOptions<RabbitMQSettings> options,
            IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMQEditGameConsumer> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "EditGameProductQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: EditGameProductQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                GameProductData? data = JsonConvert.DeserializeObject<GameProductData>(content);
                HandleMessage(data).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("EditGameProductQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(GameProductData? data)
        {
            try
            {
                if (data is not null)
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                    await gamesRepository.UpdateGameData(data);
                    scope.Dispose();
                    return;
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}