using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;

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
    }
}