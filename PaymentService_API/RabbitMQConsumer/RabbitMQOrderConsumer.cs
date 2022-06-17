using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;
using PaymentService_API.RabbitMQSender;
using PaymentService_API.Repository.IRepository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace PaymentService_API.RabbitMQConsumer
{
    public class RabbitMQOrderConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQOrderConsumer> logger;

        public RabbitMQOrderConsumer(IOptions<RabbitMQSettings> options, IRabbitMQSender rabbitMQSender,
            IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMQOrderConsumer> logger)
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
            channel.QueueDeclare(queue: "CreatePaymentQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: CreatePaymentQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                PaymentMessage? gameData = JsonConvert.DeserializeObject<PaymentMessage>(content);
                HandleMessage(gameData).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CreatePaymentQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentMessage? data)
        {
            try
            {
                if (data is not null)
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();

                    await paymentRepository.CreatePayment(data);
                    var paymentData = await paymentRepository.RequestPayment(data);
                    rabbitMQSender.SendMessage(paymentData, "PaymentUrlQueue");

                    scope.Dispose();
                    return;
                }
                rabbitMQSender.SendMessage(new PaymentUrlData(), "PaymentUrlQueue");
            }
            catch (Exception)
            {
                rabbitMQSender.SendMessage(new PaymentUrlData(), "PaymentUrlQueue");
            }
        }
    }
}