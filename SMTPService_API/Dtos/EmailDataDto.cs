using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMTPService_API.Entities;

namespace SMTPService_API.Dtos
{
    public class EmailDataDto
    {
        public EmailDataDto(string userId, string token, string receiverEmail, EmailTypes emailType)
        {
            UserId = userId;
            Token = token;
            ReceiverEmail = receiverEmail;
            EmailType = emailType;
        }

        public string UserId { get; set; }
        public string Token { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; set; }
    }
}