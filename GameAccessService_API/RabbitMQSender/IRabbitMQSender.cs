using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(object message, string queueName);
    }
}