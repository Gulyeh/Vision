using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProdcutsService_API.Extensions;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;

namespace ProductsService_API.Controllers
{
    public class GamesController : BaseApiController
    {
        private readonly IGamesRepository gamesRepository;
        public GamesController(IGamesRepository gamesRepository)
        {
            this.gamesRepository = gamesRepository;
        }

        [HttpGet("GetProductGame")]
        public async Task<ActionResult<ResponseDto>> GetGame([FromQuery] Guid GameId)
        {
            if (GameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await gamesRepository.GetGame(GameId, User.GetId()));
        }

        [HttpPut("EditProductGame")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> EditGame([FromBody] EditPackageDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await gamesRepository.EditGame(data));
        }
    }
}