using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService_API.Builders;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.RabbitMQSender;
using OrderService_API.Services.IServices;
using OrderService_API.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace OrderService_API.RabbitMQConsumer
{
    public class RabbitMQCurrencyConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly ICacheService cacheService;
        private readonly IHubContext<OrderHub> hubContext;
        private readonly IRabbitMQSender rabbitMQSender;

        public RabbitMQCurrencyConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory, IRabbitMQSender rabbitMQSender)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
            this.cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            this.rabbitMQSender = rabbitMQSender;

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "CurrencyPaymentDoneQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                CurrencyDoneDto? currencyData = JsonConvert.DeserializeObject<CurrencyDoneDto>(content);
                HandleMessage(currencyData).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("CurrencyPaymentDoneQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CurrencyDoneDto? data)
        {
            if (data is not null)
            {
                var connIds = await cacheService.TryGetFromCache(data.UserId);
                if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("CurrencyPaymentDone", new { isSuccess = data.isSuccess, amount = data.Amount });
                rabbitMQSender.SendMessage(GenerateEmail(data), "SendEmailQueue");
            }
        }

        private SMTPMessage GenerateEmail(CurrencyDoneDto data)
        {        
            var EmailBuilder = new SMTPMessageBuilder();
            EmailBuilder.SetContent("Thank you for your payment");
            EmailBuilder.SetUserId(data.UserId);
            EmailBuilder.SetReceiverEmail(data.Email);
            var email = EmailBuilder.Build();

            return email;
        }
    }
}