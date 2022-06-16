using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<ResponseDto> GetPackages();
    }
}