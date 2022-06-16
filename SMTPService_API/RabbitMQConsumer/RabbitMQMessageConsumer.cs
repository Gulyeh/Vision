using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SMTPService_API.Helpers;
using SMTPService_API.Messages;
using SMTPService_API.Repository.IRepository;
using System.Text;

namespace SMTPService_API.RabbitMQConsumer
{
    public class RabbitMQMessageConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<RabbitMQMessageConsumer> logger;
        private IConnection connection;
        private IModel channel;

        public RabbitMQMessageConsumer(IOptions<RabbitMQSettings> options, IServiceScopeFactory serviceScopeFactory, ILogger<RabbitMQMessageConsumer> logger)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "SendEmailQueue", false, false, false, arguments: null);
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                logger.LogInformation("Received message from queue: SendEmailQueue");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                EmailDataDto? emailData = JsonConvert.DeserializeObject<EmailDataDto>(content);
                HandleMessage(emailData).GetAwaiter().GetResult();
                channel.BasicAck(args.DeliveryTag, false);
            };
            channel.BasicConsume("SendEmailQueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(EmailDataDto? data)
        {
            if (data is not null)
            {
                using var scope = serviceScopeFactory.CreateScope();
                var emailRepository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();
                await emailRepository.InitializeEmail(data);
            }
        }
    }
}