using ProductsService_API.Helpers;

namespace ProductsService_API.Services.IServices
{
    public interface IBaseHttpService
    {
        void Dispose();
        Task<T?> SendAsync<T>(ApiRequest apiRequest);
    }
}