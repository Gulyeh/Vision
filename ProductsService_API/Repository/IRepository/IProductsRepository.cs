using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Dtos;

namespace ProductsService_API.Repository.IRepository
{
    public interface IProductsRepository
    {
        Task<ResponseDto> GetAllProducts();
        Task<ResponseDto> GetGameProducts(Guid gameId);
        Task<ResponseDto> AddProduct(AddProductsDto data);
        Task<ResponseDto> DeleteProduct(Guid productId);
        Task<ResponseDto> EditProduct(ProductsDto data);
    }
}