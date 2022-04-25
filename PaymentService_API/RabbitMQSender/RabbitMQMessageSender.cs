using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PaymentService_API.Helpers;
using RabbitMQ.Client;
using System.Text;

namespace PaymentService_API.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQSender
    {
        private readonly IOptions<RabbitMQSettings> options;
        private readonly ILogger<RabbitMQMessageSender> logger;
        private IConnection? connection;

        public RabbitMQMessageSender(IOptions<RabbitMQSettings> options, ILogger<RabbitMQMessageSender> logger)
        {
            this.options = options;
            this.logger = logger;
        }

        public void SendMessage(object? message, string queueName)
        {
            if (ConnectionExists())
            {
                logger.LogInformation("RabbitMQ sent message to queue: {queueName}", queueName); 
                using var channel = connection?.CreateModel();
                channel?.QueueDeclare(queue: queueName, false, false, false, arguments: null);
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
            }
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password
            };

            connection = factory.CreateConnection();
        }

        private bool ConnectionExists()
        {
            if (connection != null)
            {
                return true;
            }
            CreateConnection();
            return connection != null;
        }
    }
}