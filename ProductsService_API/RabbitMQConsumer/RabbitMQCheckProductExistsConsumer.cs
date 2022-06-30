using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProdcutsService_API.Helpers;
using ProdcutsService_API.RabbitMQSender;
using ProductsService_API.Helpers;
using ProductsService_API.Messages;
using ProductsService_API.Repository.IRepository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProdcutsService_API.RabbitMQConsumer
{
    public class RabbitMQCheckProductExistsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQCheckProductExistsConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQCheckProductExistsConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQCheckProductExistsConsumer> logger,
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
            channel.QueueDeclare(queue: "CheckProductExistsQueue", false, false, false, arguments: null);

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
                logger.LogInformation("Received message from queue: CheckProductExistsQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;
                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    CheckProductExistsDto? data = JsonConvert.DeserializeObject<CheckProductExistsDto>(content);
                    var result = HandleMessage(data).GetAwaiter().GetResult();
                    rabbitMQSender.SendMessage(result, args.BasicProperties.ReplyTo, prop);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CheckProductExistsQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> HandleMessage(CheckProductExistsDto? data)
        {
            if (data is null) return false;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
                var gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                var productsRepository = scope.ServiceProvider.GetRequiredService<IProductsRepository>();

                return data.OrderType switch
                {
                    OrderType.Currency => await currencyRepository.PackageExists(data.ProductId),
                    OrderType.Game => gamesRepository.FindGame(data.ProductId).GetAwaiter().GetResult() is not null,
                    OrderType.Product => await productsRepository.ProductExists(data.GameId, data.ProductId),
                    _ => false
                };
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}