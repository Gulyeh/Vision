using Identity_API.Dtos;

namespace Identity_API.Processors.Interfaces
{
    public interface IEmail
    {
        EmailDataDto Build();
        Task GenerateEmailData(string baseUri);
    }
}