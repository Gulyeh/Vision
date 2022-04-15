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
    public class GamesController : BaseApiController
    {
        private readonly IGamesRepository gamesRepository;
        public GamesController(IGamesRepository gamesRepository)
        {
            this.gamesRepository = gamesRepository;
        }

        [HttpGet("GetGames")]
        public async Task<ActionResult<ResponseDto>> GetGames([FromQuery] Guid? GameId = null){
            return CheckActionResult(await gamesRepository.GetGames(GameId));
        }

        [HttpPost("EditGame")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> EditGame([FromBody] GamesDto data){
            if(!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await gamesRepository.EditGame(data));
        }

        [HttpDelete("DeleteGame")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeleteGame([FromQuery] Guid GameId){
            if(GameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await gamesRepository.DeleteGame(GameId));
        }

        [HttpPost("AddGame")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddGame([FromBody] AddGamesDto data){
            if(!ModelState.IsValid) return BadRequest();
            var token = HttpContext.Request.Headers["Authorization"][0];
            return CheckActionResult(await gamesRepository.AddGame(data, token));
        }
    }
}