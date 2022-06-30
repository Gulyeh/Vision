using Microsoft.Extensions.Options;
using OrderService_API.Helpers;
using OrderService_API.RabbitMQRPC;
using OrderService_API.RabbitMQSender;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;

namespace OrderService_API.RabbitMQConsumer
{
    public class RabbitMQRPCSender : IRabbitMQRPC
    {
        private readonly ILogger<RabbitMQRPCSender> logger;
        private readonly IRabbitMQSender sender;
        private IConnection connection;
        private IModel channel;
        private bool disposed = false;
        private ConcurrentDictionary<string, TaskCompletionSource<string>> pendingMessages;

        public RabbitMQRPCSender(IOptions<RabbitMQSettings> options, ILogger<RabbitMQRPCSender> logger, IRabbitMQSender sender)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Value.Hostname,
                UserName = options.Value.Username,
                Password = options.Value.Password,
                Port = options.Value.Port
            };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            this.logger = logger;
            this.sender = sender;
            this.pendingMessages = new ConcurrentDictionary<string, TaskCompletionSource<string>>();
        }

        public Task<string> SendAsync(string queueName, object data, Guid? userId = null)
        {
            var tcs = new TaskCompletionSource<string>();
            var tempQueueName = channel.QueueDeclare().QueueName;
            ExecuteAsync(tempQueueName);

            var props = channel.CreateBasicProperties();
            props.ReplyTo = tempQueueName;
            props.CorrelationId = Guid.NewGuid().ToString();
            if (userId is not null)
            {
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("userId", userId.ToString());
            }

            this.pendingMessages[props.CorrelationId] = tcs;

            sender.SendMessage(data, queueName, props);
            return tcs.Task;
        }

        private void ExecuteAsync(string queueName)
        {
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                logger.LogInformation($"Received message from queue: {queueName}");
                var content = Encoding.UTF8.GetString(args.Body.ToArray());
                var correlationId = args.BasicProperties.CorrelationId;

                this.pendingMessages.TryRemove(correlationId, out var tcs);
                if (tcs is not null)
                {
                    channel.QueueDelete(queue: queueName);
                    tcs.SetResult(content);
                }
            };

            channel.BasicConsume(queueName, true, consumer);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                this.channel?.Dispose();
                this.connection?.Dispose();
            }

            this.disposed = true;
        }
    }
}