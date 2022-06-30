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
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository userRepository;
        private readonly ICacheService cacheService;

        public UsersController(IUserRepository userRepository, ICacheService cacheService)
        {
            this.userRepository = userRepository;
            this.cacheService = cacheService;
        }

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

        [HttpPost("ChangePhoto")]
        public async Task<ActionResult<ResponseDto>> ChangePhoto([FromBody] string image)
        {
            if (string.IsNullOrEmpty(image)) return BadRequest();
            var userId = HttpContext.User.GetId();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, await userRepository.ChangePhoto(userId, image)));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPut("ChangeCurrency")]
        public async Task<ActionResult<ResponseDto>> ChangeCurrency([FromQuery] Guid userId, [FromQuery] int amount)
        {
            if (userId == Guid.Empty) return BadRequest();
            return CheckActionResult(await userRepository.ChangeCurrency(userId, amount));
        }
    }
}