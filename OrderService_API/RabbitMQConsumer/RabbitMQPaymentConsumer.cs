using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Services.IServices;
using OrderService_API.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace OrderService_API.RabbitMQConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly ICacheService cacheService;
        private readonly IHubContext<OrderHub> hubContext;

        public RabbitMQPaymentConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory)
        {
            using var scope = serviceScopeFactory.CreateScope();
            this.hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<OrderHub>>();
            this.cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "PaymentUrlQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                PaymentUrlData? paymentUrl = JsonConvert.DeserializeObject<PaymentUrlData>(content);
                HandleMessage(paymentUrl).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("PaymentUrlQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(PaymentUrlData? data)
        {
            if(data is not null){ 
               var connIds = await cacheService.TryGetFromCache(data.userId);
               if(connIds.Count() > 0) await hubContext.Clients.Clients(connIds).SendAsync("PaymentUrl", new { paymentUrl = data.PaymentUrl });
            }
        }
    }
}