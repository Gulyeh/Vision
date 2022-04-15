using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<ResponseDto> GetProductsInGame(Guid gameId, Guid? productId = null);
        Task<ResponseDto> ProductExists(Guid gameId, Guid productId);
        Task<ResponseDto> AddProduct(AddProductsDto data, string Access_Token);
        Task<ResponseDto> DeleteProduct(Guid productId, Guid gameId);
        Task<ResponseDto> EditProduct(ProductsDto data);
    }
}