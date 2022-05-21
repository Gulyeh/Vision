using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using UsersService_API.Helpers;
using UsersService_API.Messages;
using UsersService_API.RabbitMQSender;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;

namespace UsersService_API.RabbitMQConsumer
{
    public class RabbitMQCurrencyConsumer : BackgroundService
    {
        private IConnection connection;
        private IModel channel;
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<RabbitMQCurrencyConsumer> logger;

        public RabbitMQCurrencyConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory, 
            IRabbitMQSender rabbitMQSender, ILogger<RabbitMQCurrencyConsumer> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
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
            channel.QueueDeclare(queue: "ChangeFundsQueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: ChangeFundsQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                CurrencyDto? currencyData = JsonConvert.DeserializeObject<CurrencyDto>(content);
                HandleMessage(currencyData).GetAwaiter().GetResult();

                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("ChangeFundsQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CurrencyDto? data)
        {
            if (data is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

                data.isSuccess = await currencyRepository.ChangeFunds(data);
                var userCache = await cacheService.TryGetFromCache();
                if (userCache.ContainsKey(data.UserId))
                {
                    var connIds = userCache.GetValueOrDefault(data.UserId);
                    if (connIds is not null)
                    {
                        var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<UsersHub>>();
                        await hubContext.Clients.Clients(connIds).SendAsync("CurrencyPurchased", new CurrencyPurchased { isSuccess = data.isSuccess, Amount = data.Amount});
                    }
                    if(!data.isCode) rabbitMQSender.SendMessage(data, "CurrencyPaymentDoneQueue");
                }
            }
        }
    }
}