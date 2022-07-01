using SMTPService_API.Helpers;

namespace SMTPService_API.Generator.Interfaces
{
    public interface IContent
    {
        string GenerateContent(string content);
        string GenerateSubject();
    }
}