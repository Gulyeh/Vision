using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_API.RabbitMQSender
{
    public interface IRabbitMQSender
    {
        void SendMessage(object message, string queueName);
    }
}