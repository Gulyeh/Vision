using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Statics;

namespace ProductsService_API.Controllers
{
    public class CurrencyController : BaseApiController
    {
        private readonly ICurrencyRepository currencyRepository;

        public CurrencyController(ICurrencyRepository currencyRepository)
        {
            this.currencyRepository = currencyRepository;
        }

        [HttpGet("GetPackages")]
        public async Task<ActionResult<ResponseDto>> GetPackages()
        {
            return CheckActionResult(await currencyRepository.GetPackages());
        }


        [HttpPost("AddPackage")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddPackage([FromBody] AddCurrencyDto data)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await currencyRepository.AddPackage(data));
        }
    }
}