using OrderService_API.Helpers;

namespace OrderService_API.Services.IServices
{
    public interface IBaseHttpService
    {
        void Dispose();
        Task<T?> SendAsync<T>(ApiRequest apiRequest);
    }
}