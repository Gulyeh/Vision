namespace UsersService_API.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(object message, string queueName);
    }
}