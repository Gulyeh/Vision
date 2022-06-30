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
    public class RabbitMQCouponConsumer : BackgroundService
    {
        private readonly ILogger<RabbitMQCouponConsumer> logger;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQCouponConsumer(IOptions<RabbitMQSettings> options, ILogger<RabbitMQCouponConsumer> logger, IServiceScopeFactory scopeFactory, IRabbitMQSender rabbitMQSender)
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
            channel.QueueDeclare(queue: "ProductCuponUsedQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    logger.LogInformation("RabbitMQ Consumed request from queue: ProductCuponUsedQueue");
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    UserCodeDto? gameData = JsonConvert.DeserializeObject<UserCodeDto>(content);
                    HandleMessage(gameData).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("ProductCuponUsedQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(UserCodeDto? data)
        {
            if (data is not null)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var accessRepository = scope.ServiceProvider.GetRequiredService<IAccessRepository>();
                    data.IsSuccess = await accessRepository.AddProductOrGame(data.UserId, data.GameId, data.ProductId);
                    rabbitMQSender.SendMessage(data, "AccessCouponProductQueue");
                }
                catch (Exception)
                {
                    data.IsSuccess = false;
                    rabbitMQSender.SendMessage(data, "AccessCouponProductQueue");
                    rabbitMQSender.SendMessage(new CodeFailedDto(data.UserId, data.Code), "CouponFailedQueue");
                }
            }
        }
    }
}