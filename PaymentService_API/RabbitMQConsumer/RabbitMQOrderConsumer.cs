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
        private readonly IPaymentRepository paymentRepository;

        public RabbitMQOrderConsumer(IOptions<RabbitMQSettings> options, IRabbitMQSender rabbitMQSender, IServiceScopeFactory serviceScopeFactory)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.paymentRepository = scope.ServiceProvider.GetRequiredService<IPaymentRepository>();
            this.rabbitMQSender = rabbitMQSender;

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
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
            if (data is not null)
            {
                await paymentRepository.CreatePayment(data);
                var paymentData = await paymentRepository.RequestStripePayment(data);
                rabbitMQSender.SendMessage(paymentData, "PaymentUrlQueue");
            }
            else
            {
                rabbitMQSender.SendMessage(null, "PaymentUrlQueue");
            }
        }
    }
}