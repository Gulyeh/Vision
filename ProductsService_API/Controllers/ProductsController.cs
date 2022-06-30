using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;

namespace ProductsService_API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        [HttpDelete("DeleteProduct")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> DeleteProduct([FromQuery] Guid GameId, [FromQuery] Guid ProductId)
        {
            if (GameId == Guid.Empty || ProductId == Guid.Empty) return BadRequest();
            return CheckActionResult(await productsRepository.DeleteProduct(ProductId, GameId));
        }

        [HttpPut("EditProduct")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> EditProduct([FromBody] EditPackageDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await productsRepository.EditProduct(data));
        }

        [HttpPost("AddProduct")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> AddProduct([FromBody] AddProductsDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await productsRepository.AddProduct(data));
        }
    }
}