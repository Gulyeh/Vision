using GamesDataService_API.Dtos;
using GamesDataService_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesDataService_API.Controllers
{
    public class GamesController : BaseApiController
    {
        private readonly IGamesRepository gamesRepository;
        public GamesController(IGamesRepository gamesRepository)
        {
            this.gamesRepository = gamesRepository;
        }

        [HttpGet("GetGames")]
        public async Task<ActionResult<ResponseDto>> GetAllGames()
        {
            return CheckActionResult(await gamesRepository.GetGames());
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPost("AddGame")]
        public async Task<ActionResult<ResponseDto>> AddGame([FromBody] AddGamesDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await gamesRepository.AddGame(data));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPut("EditGame")]
        public async Task<ActionResult<ResponseDto>> EditGame([FromBody] EditGameDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await gamesRepository.EditGameData(data));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpDelete("DeleteGame")]
        public async Task<ActionResult<ResponseDto>> DeleteGame([FromQuery] Guid gameId)
        {
            if (gameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await gamesRepository.DeleteGame(gameId));
        }
    }
}