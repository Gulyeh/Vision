using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Helpers;

namespace OrderService_API.Messages
{
    public class SMTPMessage
    {
        public SMTPMessage()
        {
            Content = string.Empty;
            ReceiverEmail = string.Empty;
        }

        public Guid userId { get; set; }
        public string Content { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; set; }
    }
}