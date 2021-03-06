using GameAccessService_API.Dtos;
using GameAccessService_API.Extensions;
using GameAccessService_API.Helpers;
using GameAccessService_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameAccessService_API.Controllers
{
    public class AccessController : BaseApiController
    {
        private readonly IAccessRepository accessRepository;

        public AccessController(IAccessRepository accessRepository)
        {
            this.accessRepository = accessRepository;
        }

        [HttpGet("CheckGameAccess")]
        public async Task<ActionResult<ResponseDto>> CheckAccess([FromQuery] Guid gameId)
        {
            if (gameId == Guid.Empty) return BadRequest();
            (var hasAccess, var bannedData) = await accessRepository.CheckUserAccess(gameId, User.GetId());
            return Ok(new ResponseDto(hasAccess, StatusCodes.Status200OK, bannedData));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPut("BanUserFromGame")]
        public async Task<ActionResult<ResponseDto>> BanUser([FromBody] UserAccessDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            if (User.GetId() == data.UserId) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot ban yourself" }));
            return CheckActionResult(await accessRepository.BanUserAccess(data));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpDelete("UnbanUserFromGame")]
        public async Task<ActionResult<ResponseDto>> UnbanUser([FromQuery] Guid userId, [FromQuery] Guid gameId)
        {
            if (userId == Guid.Empty || gameId == Guid.Empty) return BadRequest();
            if (User.GetId() == userId) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot unban yourself" }));
            return CheckActionResult(await accessRepository.UnbanUserAccess(userId, gameId));
        }

        [HttpGet("CheckUserIsBanned")]
        public async Task<ActionResult<ResponseDto>> CheckUserIsBanned([FromQuery] Guid userId, [FromQuery] Guid gameId)
        {
            if (userId == Guid.Empty || gameId == Guid.Empty) return BadRequest();
            if (User.GetId() == userId) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot check ban for yourself" }));
            return CheckActionResult(new ResponseDto(true, StatusCodes.Status200OK, await accessRepository.CheckUserIsBanned(userId, gameId)));
        }

        [HttpGet("BoughtGame")]
        public async Task<ActionResult<ResponseDto>> CheckUserBoughtGame([FromQuery] Guid gameId)
        {
            if (gameId == Guid.Empty) return BadRequest();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, new HasAccess(await accessRepository.CheckUserHasGame(gameId, User.GetId()))));
        }

        [HttpGet("OwnsProduct")]
        public async Task<ActionResult<ResponseDto>> CheckUserOwnsProduct([FromQuery] Guid productId, [FromQuery] Guid gameId)
        {
            if (productId == Guid.Empty) return BadRequest();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, new HasAccess(await accessRepository.CheckUserHasProduct(productId, gameId, User.GetId()))));
        }

        [HttpPost("GiveUserProduct")]
        public async Task<ActionResult<ResponseDto>> GiveUserProduct([FromBody] GiveUserProductDto data)
        {
            if (!ModelState.IsValid) return BadRequest();

            var product = data.ProductId != Guid.Empty ? data.ProductId : data.GameId;
            var game = data.ProductId != Guid.Empty ? data.GameId : Guid.Empty;

            var gaveAccess = await accessRepository.AddProductOrGame(data.UserId, game, product);
            var item = data.ProductId == Guid.Empty ? "game" : "product";

            if (!gaveAccess) return CheckActionResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { $"Could not give access to {item}" }));
            return CheckActionResult(new ResponseDto(true, StatusCodes.Status200OK, new[] { $"Gave access to {item}" }));
        }
    }
}