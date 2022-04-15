using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Statics;

namespace ProductsService_API.Controllers
{
    public class ProductsContoller : BaseApiController
    {
        private readonly IProductsRepository productsRepository;

        public ProductsContoller(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        [HttpGet("GetProductsInGame")]
        public async Task<ActionResult<ResponseDto>> GetProductsInGame([FromQuery] Guid GameId, [FromQuery] Guid? ProductId = null){
            if(GameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await productsRepository.GetProductsInGame(GameId, ProductId));
        }

        [HttpDelete("DeleteProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeleteProduct([FromQuery] Guid GameId, [FromQuery] Guid ProductId){
            if(GameId == Guid.Empty || ProductId == Guid.Empty) return BadRequest();
            return CheckActionResult(await productsRepository.DeleteProduct(ProductId, GameId));
        }

        [HttpPost("EditProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> EditProduct([FromBody] ProductsDto data){
            if(!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await productsRepository.EditProduct(data));
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddProduct([FromBody] AddProductsDto data){
            if(!ModelState.IsValid) return BadRequest();
            var token = HttpContext.Request.Headers["Authorization"][0];
            return CheckActionResult(await productsRepository.AddProduct(data, token));
        }
    }
}