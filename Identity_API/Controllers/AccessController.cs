using Identity_API.Dtos;
using Identity_API.Extensions;
using Identity_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_API.Controllers
{
    [Authorize]
    public class AccessController : BaseControllerApi
    {
        private readonly IAccessRepository accessRepository;

        public AccessController(IAccessRepository accessRepository)
        {
            this.accessRepository = accessRepository;
        }

        [HttpPost("BanUser")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> BanUserAccount([FromBody] BannedUsersDto data)
        {
            if (!ModelState.IsValid) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            if (data.UserId == User.GetId()) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot ban yourself" });
            return CheckActionResult(await accessRepository.BanUser(data));
        }

        [HttpDelete("UnbanUser")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> UnbanUserAccount([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            return CheckActionResult(await accessRepository.UnbanUser(userId));
        }


        [HttpGet("GetServerData")]
        public async Task<ActionResult<ResponseDto>> GetServerData([FromQuery] Guid sessionToken)
        {
            var userId = HttpContext.User.GetId();
            return CheckActionResult(await accessRepository.GetServerData(sessionToken, userId));
        }

        [HttpGet("GetRoles")]
        public async Task<ActionResult<ResponseDto>> GetRoles()
        {
            return CheckActionResult(new ResponseDto(true, StatusCodes.Status200OK, await accessRepository.GetRoles()));
        }

        [HttpPost("ChangeUserRole")]
        public async Task<ActionResult<ResponseDto>> ChangeUserRole([FromQuery] Guid userId, [FromQuery] string role)
        {
            if(userId == Guid.Empty || string.IsNullOrWhiteSpace(role)) return BadRequest();
            var requester = User.GetId();
            return CheckActionResult(await accessRepository.ChangeUserRole(userId, role, requester));
        }
    }
}