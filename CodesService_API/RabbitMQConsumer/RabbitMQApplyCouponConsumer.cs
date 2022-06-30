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
    public class RabbitMQApplyCouponConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<RabbitMQApplyCouponConsumer> logger;
        private readonly IRabbitMQSender rabbitMQSender;
        private IConnection connection;
        private IModel channel;

        public RabbitMQApplyCouponConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory scopeFactory, ILogger<RabbitMQApplyCouponConsumer> logger,
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
            channel.QueueDeclare(queue: "ApplyCouponQueue", false, false, false, arguments: null);

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
                logger.LogInformation("Received message from queue: ApplyCouponQueue");
                var prop = channel.CreateBasicProperties();
                prop.CorrelationId = args.BasicProperties.CorrelationId;

                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    ApplyCouponDto? data = JsonConvert.DeserializeObject<ApplyCouponDto>(content);
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
            channel.BasicConsume("ApplyCouponQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task<object?> HandleMessage(ApplyCouponDto? data)
        {
            if (data is null) return null;
            try
            {
                using var scope = scopeFactory.CreateScope();
                var codesRepository = scope.ServiceProvider.GetRequiredService<ICodesRepository>();
                var response = await codesRepository.ApplyCode(data.Coupon, data.UserId, data.CodeType);
                return response.Response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}