using GamesDataService_API.Dtos;
using GamesDataService_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GamesDataService_API.Controllers
{
    public class NewsController : BaseApiController
    {
        private readonly INewsRepository newsRepository;

        public NewsController(INewsRepository newsRepository)
        {
            this.newsRepository = newsRepository;
        }

        [HttpGet("GetNews")]
        public async Task<ActionResult<ResponseDto>> GetGameNews([FromQuery] Guid gameId)
        {
            if (gameId == Guid.Empty) return BadRequest();
            return CheckActionResult(await newsRepository.GetGameNews(gameId));
        }

        [HttpPost("AddNews")]
        [Authorize(Policy = "HasAdminOrModRole")]
        public async Task<ActionResult<ResponseDto>> AddGameNews([FromBody] AddNewsDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await newsRepository.AddNews(data));
        }

        [HttpDelete("DeleteNews")]
        [Authorize(Policy = "HasAdminOrModRole")]
        public async Task<ActionResult<ResponseDto>> DeleteGameNews([FromQuery] Guid newsId, [FromQuery] Guid gameId)
        {
            if (newsId == Guid.Empty) return BadRequest();
            return CheckActionResult(await newsRepository.DeleteNews(newsId, gameId));
        }

        [HttpPost("EditNews")]
        [Authorize(Policy = "HasAdminOrModRole")]
        public async Task<ActionResult<ResponseDto>> EditGameNews([FromBody] NewsDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await newsRepository.EditNews(data));
        }
    }
}