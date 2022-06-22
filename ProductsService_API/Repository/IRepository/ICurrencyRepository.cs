using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<ResponseDto> GetPackages();
        Task<ResponseDto> AddPackage(AddCurrencyDto data);
        Task<ResponseDto> DeletePackage(Guid packageId);
        Task<ResponseDto> EditPackage(EditCurrencyDto data);
    }
}