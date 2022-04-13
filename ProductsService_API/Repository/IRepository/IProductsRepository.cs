using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<ResponseDto> GetAllProducts();
        Task<ResponseDto> GetGame(Guid gameId);
        Task<ResponseDto> AddProduct(AddProductsDto data, string Access_Token);
        Task<ResponseDto> DeleteProduct(Guid productId);
        Task<ResponseDto> EditProduct(ProductsDto data);
    }
}