using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Dtos;
using GamesDataService_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GamesDataService_API.Statics;

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
        public async Task<ActionResult<ResponseDto>> GetAllGames(){
            return CheckActionResult(await gamesRepository.GetGames());
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPost("AddGame")]
        public async Task<ActionResult<ResponseDto>> AddGame([FromBody]AddGamesDto data){
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await gamesRepository.AddGame(data));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPost("EditGame")]
        public async Task<ActionResult<ResponseDto>> EditGame([FromBody]GamesDto data){
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await gamesRepository.EditGameData(data));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpDelete("DeleteGame")]
        public async Task<ActionResult<ResponseDto>> DeleteGame([FromQuery]Guid gameId){
            if(gameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await gamesRepository.DeleteGame(gameId));
        }      
    }
}