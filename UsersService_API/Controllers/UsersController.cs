using GameAccessService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UsersService_API.Dtos;
using UsersService_API.Helpers;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;
using UsersService_API.Statics;

namespace UsersService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly ICacheService cacheService;
        private readonly IHubContext<UsersHub> hubContext;

        public UsersController(IUserRepository userRepository, ICacheService cacheService, IHubContext<UsersHub> hubContext)
        {
            this.userRepository = userRepository;
            this.cacheService = cacheService;
            this.hubContext = hubContext;
        }

        [AllowAnonymous]
        [HttpGet("UserExists")]
        public async Task<ActionResult<ResponseDto>> UserExists([FromQuery] Guid userId)
        {
            return Ok(await userRepository.UserExists(userId));
        }

        [Authorize]
        [HttpGet("IsUserBlocked")]
        public async Task<ActionResult<ResponseDto>> IsUserBlocked([FromQuery] Guid userId, [FromQuery] Guid user2Id)
        {
            return Ok(await userRepository.IsUserBlocked(userId, user2Id));
        }

        [Authorize]
        [HttpGet("FindUser")]
        public async Task<ActionResult<ResponseDto>> FindUsers([FromQuery] string containsString)
        {
            if(string.IsNullOrWhiteSpace(containsString)) return BadRequest();
            var userId = HttpContext.User.GetId();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, await userRepository.FindUsers(containsString, userId)));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpGet("FindDetailedUser")]
        public async Task<ActionResult<ResponseDto>> FindDetailedUser([FromQuery] string containsString)
        {
            if(string.IsNullOrWhiteSpace(containsString)) return BadRequest();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, await userRepository.FindDetailedUsers(containsString)));
        }

        [Authorize]
        [HttpPost("ChangePhoto")]
        public async Task<ActionResult<ResponseDto>> ChangePhoto([FromBody] string image)
        {
            if (string.IsNullOrEmpty(image)) return BadRequest();
            var userId = HttpContext.User.GetId();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, await userRepository.ChangePhoto(userId, image)));
        }

        [Authorize]
        [HttpGet("MessageNotification")]
        public async Task<ActionResult> SendUserMessageNotification([FromQuery] Guid receiverId, [FromQuery] Guid senderId)
        {
            var response = await userRepository.UserExists(receiverId);
            if (response.Response is true)
            {
                var connIds = await cacheService.TryGetFromCache(HubTypes.Users);
                if (connIds.ContainsKey(receiverId))
                {
                    var userIds = connIds.GetValueOrDefault(receiverId);
                    if (userIds is not null) await hubContext.Clients.Clients(userIds).SendAsync("ChatNotification", senderId);
                }
            }
            return Ok();
        }
    }
}