using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;
using OrderService_API.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;


namespace OrderService_API.RabbitMQConsumer
{
    public class RabbitMQPaymentCompletedConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<RabbitMQPaymentCompletedConsumer> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public RabbitMQPaymentCompletedConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory,
            IRabbitMQSender rabbitMQSender, ILogger<RabbitMQPaymentCompletedConsumer> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.rabbitMQSender = rabbitMQSender;
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
            channel.QueueDeclare(queue: "PaymentQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received data from queue: PaymentQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                PaymentCompleted? paymentCompleted = JsonConvert.DeserializeObject<PaymentCompleted>(content);
                HandleMessage(paymentCompleted).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("PaymentQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentCompleted? data)
        {
            if (data is not null)
            {
                try
                {
                    if (data.IsSuccess)
                    {
                        using var scope = serviceScopeFactory.CreateScope();
                        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
                        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
                        var productService = scope.ServiceProvider.GetRequiredService<IProductsService>();
                        var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                        var orderTypeProcessor = scope.ServiceProvider.GetRequiredService<IOrderTypeProcessor>();

                        var order = await orderRepository.GetOrder(data.OrderId);
                        await orderRepository.ChangeOrderStatus(data.OrderId, data.IsSuccess, data.PaymentId);
                        var orderProccessor = orderTypeProcessor.GetOrderOfType(order.OrderType);

                        if (orderProccessor is null)
                        {
                            await PaymentNotCompleted(data);
                            return;
                        }

                        var completed = await orderProccessor.PaymentCompleted(data, order);
                        if (!completed)
                        {
                            await PaymentNotCompleted(data);
                            return;
                        }

                        rabbitMQSender.SendMessage(new EmailDataDto(data.UserId, order.Title, data.Email), "SendEmailQueue");
                        scope.Dispose();
                    }
                    else
                    {
                        await PaymentNotCompleted(data);
                    }
                }
                catch (Exception)
                {
                    await PaymentNotCompleted(data);
                }
            }
        }

        private async Task PaymentNotCompleted(PaymentCompleted data)
        {
            using var scope = serviceScopeFactory.CreateScope();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            var connIds = await cacheService.TryGetFromCache(data.UserId);
            if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("PaymentFailed");

            scope.Dispose();
        }
    }
}