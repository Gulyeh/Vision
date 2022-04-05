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
    public class ProductsController : BaseApiController
    {
        private readonly IProductsRepository productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        [HttpGet("GetAllProducts")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> GetProducts(){
            return CheckActionResult(await productsRepository.GetAllProducts());
        }

        [HttpGet("GetGameProducts")]
        public async Task<ActionResult<ResponseDto>> GetGameProducts([FromQuery]Guid gameId){
            if(gameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await productsRepository.GetGameProducts(gameId));
        }

        [HttpDelete("DeleteProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeleteProduct([FromQuery]Guid productId){
            if(productId == Guid.Empty) return BadRequest();
            return CheckActionResult(await productsRepository.DeleteProduct(productId));
        }

        [HttpPost("EditProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> EditProduct([FromBody]ProductsDto data){
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await productsRepository.EditProduct(data));
        }

        [HttpPost("AddProduct")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddProduct([FromBody]AddProductsDto data){
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await productsRepository.AddProduct(data));
        }
    }
}