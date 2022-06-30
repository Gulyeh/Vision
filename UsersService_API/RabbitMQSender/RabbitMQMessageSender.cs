using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using UsersService_API.Helpers;

namespace UsersService_API.RabbitMQSender
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

        public void SendMessage(object? message, string queueName, IBasicProperties? properties = null)
        {
            if (ConnectionExists())
            {
                using var channel = connection?.CreateModel();
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: properties, body: body);
                logger.LogInformation("RabbitMQ sent message to queue: {queueName}", queueName);
            }
        }

        private void CreateConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
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