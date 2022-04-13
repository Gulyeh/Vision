using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using UsersService_API.Helpers;

namespace UsersService_API.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQSender
    {
        private readonly IOptions<RabbitMQSettings> options;
        private IConnection? connection;

        public RabbitMQMessageSender(IOptions<RabbitMQSettings> options)
        {
            this.options = options;
        }

        public void SendMessage(object message, string queueName)
        {
            if (ConnectionExists())
            {
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
            if(connection != null)
            {
                return true;
            }
            CreateConnection();
            return connection != null;
        }
    }
}