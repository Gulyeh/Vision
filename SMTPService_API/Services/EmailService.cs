using MailKit.Net.Smtp;
using MimeKit;
using SMTPService_API.Helpers;
using SMTPService_API.Messages;
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
            try
            {
                var emailToSend = new MimeMessage();
                emailToSend.From.Add(MailboxAddress.Parse(config["SenderName"]));
                emailToSend.To.Add(MailboxAddress.Parse(data.ReceiverEmail));
                emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = data.Content };
                switch (data.EmailType)
                {
                    case EmailTypes.ResetPassword:
                        emailToSend.Subject = "Reset Password";
                        break;
                    case EmailTypes.Confirmation:
                        emailToSend.Subject = "Account Confirmation";
                        break;
                    case EmailTypes.Payment:
                        emailToSend.Subject = "Payment Confirmation";
                        break;
                    default:
                        break;
                }

                using var emailClient = new SmtpClient();
                await emailClient.ConnectAsync(config["SMTPServer"], int.Parse(config["SMTPPort"]), MailKit.Security.SecureSocketOptions.StartTls);
                await emailClient.AuthenticateAsync(config["SenderEmail"], config["SenderPassword"]);
                await emailClient.SendAsync(emailToSend);
                await emailClient.DisconnectAsync(true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}