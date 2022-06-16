using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Statics;

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
            var token = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(token)) return new ResponseDto(false, StatusCodes.Status400BadRequest, false);
            return CheckActionResult(await gamesRepository.GetGame(GameId, token));
        }

        [HttpPut("EditProductGame")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> EditGame([FromBody] GamesDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await gamesRepository.EditGame(data));
        }

        [HttpDelete("DeleteProductGame")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeleteGame([FromQuery] Guid GameId)
        {
            if (GameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await gamesRepository.DeleteGame(GameId));
        }

        [HttpPost("AddProductGame")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddGame([FromBody] AddGamesDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            var token = HttpContext.Request.Headers["Authorization"][0];
            return CheckActionResult(await gamesRepository.AddGame(data, token));
        }
    }
}