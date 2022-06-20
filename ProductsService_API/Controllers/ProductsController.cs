using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Statics;

namespace ProductsService_API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        [HttpGet("GetGameProducts")]
        public async Task<ActionResult<ResponseDto>> GetGameProducts([FromQuery] Guid GameId, [FromQuery] Guid ProductId)
        {
            if (GameId == Guid.Empty) return BadRequest();
            var token = HttpContext.Request.Headers["Authorization"][0];
            return CheckActionResult(await productsRepository.GetProduct(GameId, ProductId, token));
        }

        [HttpDelete("DeleteProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeleteProduct([FromQuery] Guid GameId, [FromQuery] Guid ProductId)
        {
            if (GameId == Guid.Empty || ProductId == Guid.Empty) return BadRequest();
            return CheckActionResult(await productsRepository.DeleteProduct(ProductId, GameId));
        }

        [HttpPut("EditProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> EditProduct([FromBody] ProductsDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await productsRepository.EditProduct(data));
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddProduct([FromBody] AddProductsDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var token = HttpContext.Request.Headers["Authorization"][0];
            return CheckActionResult(await productsRepository.AddProduct(data, token));
        }
    }
}