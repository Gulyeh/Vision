using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using SMTPService_API.Generator.Interfaces;
using SMTPService_API.Helpers;
using SMTPService_API.Messages;
using SMTPService_API.Services.IServices;

namespace SMTPService_API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;
        private readonly IEmailGenerator emailGenerator;

        public EmailService(IConfiguration config, IEmailGenerator emailGenerator)
        {
            this.config = config.GetSection("SMTPData");
            this.emailGenerator = emailGenerator;
        }

        public async Task<bool> SendEmail(EmailDataDto data)
        {
            try
            {
                var generator = emailGenerator.GetType(data.EmailType);
                if (generator is null) return false;

                MailjetClient client = new MailjetClient(config["SenderAccount"], config["SenderPassword"]);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                .Property(Send.FromEmail, config["SenderEmail"])
                .Property(Send.FromName, "Vision")
                .Property(Send.Recipients, new JArray {
                    new JObject {
                        {"Email", data.ReceiverEmail}
                    }
                })
                .Property(Send.HtmlPart, generator.GenerateContent(data.Content));

                switch (data.EmailType)
                {
                    case EmailTypes.ResetPassword:
                        request.Property(Send.Subject, "Reset Password");
                        break;
                    case EmailTypes.Confirmation:
                        request.Property(Send.Subject, "Account Confirmation");
                        break;
                    case EmailTypes.Payment:
                        request.Property(Send.Subject, "Payment Confirmation");
                        break;
                    default:
                        break;
                }

                MailjetResponse response = await client.PostAsync(request);
                if (response.IsSuccessStatusCode) return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}