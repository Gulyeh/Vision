using GameAccessService_API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using UsersService_API.Dtos;
using UsersService_API.Repository.IRepository;
using UsersService_API.Services.IServices;
using UsersService_API.SignalR;

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

        [Authorize]
        [HttpGet("FindUser")]
        public async Task<ActionResult<ResponseDto>> FindUsers([FromQuery] string containsString)
        {
            if (string.IsNullOrWhiteSpace(containsString)) return BadRequest();
            var userId = HttpContext.User.GetId();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, await userRepository.FindUsers(containsString, userId)));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpGet("FindDetailedUser")]
        public async Task<ActionResult<ResponseDto>> FindDetailedUser([FromQuery] string containsString)
        {
            if (string.IsNullOrWhiteSpace(containsString)) return BadRequest();
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
    }
}