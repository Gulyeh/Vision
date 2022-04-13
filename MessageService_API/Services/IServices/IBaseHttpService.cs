using MessageService_API.Helpers;

namespace MessageService_API.Services.IServices
{
    public interface IBaseHttpService
    {
        void Dispose();
        Task<T?> SendAsync<T>(ApiRequest apiRequest);
    }
}