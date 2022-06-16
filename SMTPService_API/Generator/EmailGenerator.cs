using SMTPService_API.Generator.Interfaces;
using SMTPService_API.Helpers;

namespace SMTPService_API.Generator
{
    public class EmailGenerator : IEmailGenerator
    {
        public IContent? GetType(EmailTypes type)
        {
            return type switch
            {
                EmailTypes.Confirmation => new ConfirmationContent(),
                EmailTypes.Payment => new PaymentContent(),
                EmailTypes.ResetPassword => new ResetPasswordContent(),
                _ => null
            };
        }
    }
}