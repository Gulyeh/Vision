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
                Password = options.Value.Password,
                Port = options.Value.Port
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
                List<string>? connIds = null;
                using var scope = serviceScopeFactory.CreateScope();
                var currencyRepository = scope.ServiceProvider.GetRequiredService<ICurrencyRepository>();
                var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
                var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<UsersHub>>();

                try
                {
                    data.IsSuccess = await currencyRepository.ChangeFunds(data);
                    if (!data.IsCode) rabbitMQSender.SendMessage(data, "CurrencyPaymentDoneQueue");
                    else
                    {
                        var userCache = await cacheService.TryGetFromCache(HubTypes.Users);
                        if (userCache.ContainsKey(data.UserId))
                        {
                            connIds = userCache.GetValueOrDefault(data.UserId);
                            if (connIds is not null) await hubContext.Clients.Clients(connIds).SendAsync("CurrencyCodeUsed", new CurrencyPurchased(data.IsSuccess, data.Amount));
                        }
                    }
                }
                catch (Exception)
                {
                    data.IsSuccess = false;
                    if (!data.IsCode) rabbitMQSender.SendMessage(data, "CurrencyPaymentDoneQueue");
                    else
                    {
                        if (connIds is not null && connIds.Count > 0) await hubContext.Clients.Clients(connIds).SendAsync("CodeFailed", new[] { "Currency coupon could not be applied correctly" });
                        rabbitMQSender.SendMessage(new CodeFailedDto(data.UserId, data.Code), "CouponFailedQueue");
                    }
                }
            }
        }
    }
}