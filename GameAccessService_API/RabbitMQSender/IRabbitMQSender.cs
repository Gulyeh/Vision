using RabbitMQ.Client;

namespace GameAccessService_API.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(object? message, string queueName, IBasicProperties? props = null);
    }
}