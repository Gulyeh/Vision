using RabbitMQ.Client;

namespace MessageService_API.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(object? message, string queueName, IBasicProperties? properties = null);
    }
}