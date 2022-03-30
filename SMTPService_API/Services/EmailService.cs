using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using SMTPService_API.Dtos;
using SMTPService_API.Services.IServices;

namespace SMTPService_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;

        public EmailService(IConfiguration config)
        {
            this.config = config.GetSection("SMTPData");
        }

        public async Task<bool> SendEmail(EmailDataDto data)
        {
            try{
                var emailToSend = new MimeMessage();
                emailToSend.From.Add(MailboxAddress.Parse(config["SenderName"]));
                emailToSend.To.Add(MailboxAddress.Parse(data.ReceiverEmail));
                emailToSend.Subject = data.EmailType.ToString();
                emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = data.Token };

                using var emailClient = new SmtpClient();
                await emailClient.ConnectAsync(config["SMTPServer"], int.Parse(config["SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await emailClient.AuthenticateAsync(config["SenderEmail"], config["SenderPassword"]);
                await emailClient.SendAsync(emailToSend);
                await emailClient.DisconnectAsync(true);
                return true;
            }catch(Exception){
                return false;
            }
        }
    }
}