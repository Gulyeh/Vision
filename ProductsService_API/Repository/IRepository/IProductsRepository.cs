using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<ResponseDto> GetProduct(Guid gameId, Guid productId, string Access_Token);
        Task<ResponseDto> ProductExists(Guid gameId, Guid productId);
        Task<ResponseDto> AddProduct(AddProductsDto data, string Access_Token);
        Task<ResponseDto> DeleteProduct(Guid productId, Guid gameId);
        Task<ResponseDto> EditProduct(EditPackageDto data);
    }
}