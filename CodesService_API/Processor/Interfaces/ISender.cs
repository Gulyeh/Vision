namespace CodesService_API.Processor.Interfaces
{
    public interface ISender
    {
        void SendRabbitMQMessage(Guid userId, Guid? gameId, string codeValue, string code);
    }
}