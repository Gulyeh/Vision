using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Services.IServices;
using OrderService_API.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace OrderService_API.RabbitMQConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQPaymentConsumer> logger;

        public RabbitMQPaymentConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            ILogger<RabbitMQPaymentConsumer> logger)
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
            channel.QueueDeclare(queue: "PaymentUrlQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    logger.LogInformation("Received data from queue: PaymentUrlQueue");
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    PaymentUrlData? paymentUrl = JsonConvert.DeserializeObject<PaymentUrlData>(content);
                    HandleMessage(paymentUrl).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.ToString());
                }
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("PaymentUrlQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentUrlData? data)
        {
            if (data is not null)
            {
                List<string> connIds = new();
                using var scope = serviceScopeFactory.CreateScope();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
                try
                {
                    var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                    connIds = await cacheService.TryGetFromCache(data.userId);
                    if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("PaymentUrl", data.PaymentUrl);
                }
                catch (Exception)
                {
                    if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("PaymentUrl", data.PaymentUrl);
                }
            }
        }
    }
}