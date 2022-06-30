using CodesService_API.Messages;
using CodesService_API.Processor.Interfaces;
using CodesService_API.RabbitMQSender;

namespace CodesService_API.Processor
{
    public class CurrencyType : ISender
    {
        private readonly IRabbitMQSender rabbitMQSender;

        public CurrencyType(IRabbitMQSender rabbitMQSender)
        {
            this.rabbitMQSender = rabbitMQSender;
        }

        public void SendRabbitMQMessage(Guid userId, Guid? gameId, string codeValue, string code)
        {
            int.TryParse(codeValue, out int amount);
            rabbitMQSender.SendMessage(new CurrencyDto(userId, amount, true, code), "ChangeFundsQueue");
        }
    }
}