using CodesService_API.Helpers;
using CodesService_API.Messages;
using CodesService_API.RabbitMQSender;
using CodesService_API.Repository.IRepository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace CodesService_API.RabbitMQConsumer
{
    public class RabbitMQCouponFailedConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQCouponFailedConsumer> logger;

        public RabbitMQCouponFailedConsumer(IOptions<RabbitMQSettings> options, IRabbitMQSender rabbitMQSender,
            IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMQCouponFailedConsumer> logger)
        {
            this.rabbitMQSender = rabbitMQSender;
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
            channel.QueueDeclare(queue: "CouponFailedQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    logger.LogInformation("Received message from queue: CouponFailedQueue");
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    CodeFailedDto? codeData = JsonConvert.DeserializeObject<CodeFailedDto>(content);
                    HandleMessage(codeData).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CouponFailedQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CodeFailedDto? data)
        {
            if (data is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var codesRepository = scope.ServiceProvider.GetRequiredService<ICodesRepository>();
                await codesRepository.RemoveUsedCode(data.Code, data.UserId);
            }
        }
    }
}