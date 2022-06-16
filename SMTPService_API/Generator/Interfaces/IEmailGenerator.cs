using SMTPService_API.Helpers;

namespace SMTPService_API.Generator.Interfaces
{
    public interface IEmailGenerator
    {
        IContent? GetType(EmailTypes type);
    }
}