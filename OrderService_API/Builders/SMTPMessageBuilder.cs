using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Helpers;
using OrderService_API.Messages;

namespace OrderService_API.Builders
{
    public class SMTPMessageBuilder
    {
        private SMTPMessage message = new SMTPMessage();
        public SMTPMessageBuilder()
        {
            message.EmailType = EmailTypes.Payment;
        }

        public SMTPMessage Build(){
            return message;
        }

        public void SetReceiverEmail(string email){
            message.ReceiverEmail = email;
        }

        public void SetUserId(Guid userId){
            message.userId = userId;
        }

        public void SetContent(string content){
            message.Content = content;
        }
    }
}