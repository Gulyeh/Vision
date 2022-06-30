using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ProdcutsService_API.Helpers;
using ProdcutsService_API.Messages;
using ProdcutsService_API.RabbitMQSender;
using ProductsService_API.Helpers;
using ProductsService_API.Repository.IRepository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProdcutsService_API.RabbitMQConsumer
{
    public class RabbitMQGetProductsConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQGetProductsConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQGetProductsConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQGetProductsConsumer> logger,
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
            channel.QueueDeclare(queue: "GetProductsQueue", false, false, false, arguments: null);

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

                logger.LogInformation("Received message from queue: GetProductsQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;

                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    GetProductDto? data = JsonConvert.DeserializeObject<GetProductDto>(content);
                    var result = HandleMessage(data).GetAwaiter().GetResult();
                    rabbitMQSender.SendMessage(result, args.BasicProperties.ReplyTo, prop);
                }
                catch (Exception)
                {
                    rabbitMQSender.SendMessage(null, args.BasicProperties.ReplyTo, prop);
                }

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("GetProductsQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<object?> HandleMessage(GetProductDto? data)
        {
            if (data is null) return null;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
                var gamesRepository = scope.ServiceProvider.GetRequiredService<IGamesRepository>();
                var productsRepository = scope.ServiceProvider.GetRequiredService<IProductsRepository>();

                return data.OrderType switch
                {
                    OrderType.Currency => await currencyRepository.GetPackages(),
                    OrderType.Game => gamesRepository.GetGame(data.ProductId, data.UserId).GetAwaiter().GetResult().Response,
                    OrderType.Product => await productsRepository.GetProduct(data.GameId, data.ProductId, data.UserId),
                    _ => null
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}