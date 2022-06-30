using RabbitMQ.Client;

namespace ProdcutsService_API.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(object? message, string queueName, IBasicProperties? props = null);
    }
}