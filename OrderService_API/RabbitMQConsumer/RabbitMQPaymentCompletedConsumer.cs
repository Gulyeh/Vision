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
        private readonly ICacheService cacheService;
        private readonly IHubContext<OrderHub> hubContext;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<RabbitMQPaymentCompletedConsumer> logger;
        private readonly IOrderRepository orderRepository;
        private readonly IProductsService productService;

        public RabbitMQPaymentCompletedConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory, 
            IRabbitMQSender rabbitMQSender, ILogger<RabbitMQPaymentCompletedConsumer> logger)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
            this.orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            this.productService = scope.ServiceProvider.GetRequiredService<IProductsService>();
            this.cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
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
                if (data.isSuccess)
                {
                    var order = await orderRepository.GetOrder(data.orderId);
                    await orderRepository.ChangeOrderStatus(data.orderId, data.isSuccess);
                    var orderProccessor = new OrderTypeProcessor(order.OrderType, orderRepository, rabbitMQSender, productService).CreateOrder();
                    if(orderProccessor is null) {
                        await PaymentNotCompleted(data);
                        return;
                    }
                    var completed = await orderProccessor.PaymentCompleted(data, order);
                    if(!completed) await PaymentNotCompleted(data);
                }
                else
                {
                    await PaymentNotCompleted(data);
                }
            }
        }

        private async Task PaymentNotCompleted(PaymentCompleted data){
            var connIds = await cacheService.TryGetFromCache(data.userId);
            if (connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("PaymentDone", new { isSuccess = false });
        }
    }
}