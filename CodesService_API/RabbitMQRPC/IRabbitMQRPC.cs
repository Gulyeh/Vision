namespace CodesService_API.RabbitMQRPC
{
    public interface IRabbitMQRPC : IDisposable
    {
        public Task<string> SendAsync(string queueName, object data, Guid? userId = null);
    }
}