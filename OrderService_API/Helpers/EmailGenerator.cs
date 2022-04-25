using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Builders;
using OrderService_API.Messages;

namespace OrderService_API.Helpers
{
    public class EmailGenerator<T> where T : BaseEmailData
    {
        private SMTPMessageBuilder builder = new();
        public EmailGenerator(T data)
        {
            builder.SetContent("Thank you for your payment");
            builder.SetUserId(data.UserId);
            builder.SetReceiverEmail(data.Email);
        }

        public SMTPMessage Generate(){
            return builder.Build();
        }
    }
}