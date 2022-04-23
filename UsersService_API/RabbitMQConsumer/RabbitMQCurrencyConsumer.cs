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
        private readonly IHubContext<UsersHub> hubContext;
        private readonly ICacheService cacheService;
        private readonly ICurrencyRepository currencyRepository;
        private IConnection connection;
        private IModel channel;
        private readonly IRabbitMQSender rabbitMQSender;

        public RabbitMQCurrencyConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory, IRabbitMQSender rabbitMQSender)
        {
            this.rabbitMQSender = rabbitMQSender;
            using var scope = serviceScopeFactory.CreateScope();
            this.hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<UsersHub>>();
            this.currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
            this.cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

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
                data.isSuccess = await currencyRepository.ChangeFunds(data);
                var userCache = await cacheService.TryGetFromCache();
                if (userCache.ContainsKey(data.UserId))
                {
                    var connIds = userCache.GetValueOrDefault(data.UserId);
                    if (connIds is not null)
                    {
                        await hubContext.Clients.Clients(connIds).SendAsync("CurrencyPurchased", new CurrencyPurchased { isSuccess = data.isSuccess, Amount = data.Amount});
                        if(!data.isCode) rabbitMQSender.SendMessage(data, "CurrencyPaymentDoneQueue");
                    }
                }
            }
        }
    }
}