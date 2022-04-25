using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.RabbitMQSender;

namespace CodesService_API.Processor.Interfaces
{
    public interface ISender : IResponder
    {
        void SendRabbitMQMessage(Guid userId, Guid? gameId = null, string? productId = null);
    }
}