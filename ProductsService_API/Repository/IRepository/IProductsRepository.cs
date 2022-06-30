using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<ProductsDto?> GetProduct(Guid gameId, Guid productId, Guid userId);
        Task<bool> ProductExists(Guid gameId, Guid productId);
        Task<ResponseDto> AddProduct(AddProductsDto data);
        Task<ResponseDto> DeleteProduct(Guid productId, Guid gameId);
        Task<ResponseDto> EditProduct(EditPackageDto data);
    }
}