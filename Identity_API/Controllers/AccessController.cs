using Identity_API.Dtos;
using Identity_API.Extensions;
using Identity_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_API.Controllers
{
    [Authorize(Policy = "HasAdminRole")]
    public class AccessController : BaseControllerApi
    {
        private readonly IAccessRepository accessRepository;

        public AccessController(IAccessRepository accessRepository)
        {
            this.accessRepository = accessRepository;
        }

        [HttpPost("BanUser")]
        public async Task<ActionResult<ResponseDto>> BanUserAccount([FromBody] BannedUsersDto data)
        {
            if (!ModelState.IsValid) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            if (data.UserId == User.GetId()) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot ban yourself" });
            return CheckActionResult(await accessRepository.BanUser(data));
        }

        [HttpPost("UnbanUser")]
        public async Task<ActionResult<ResponseDto>> UnbanUserAccount([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            return CheckActionResult(await accessRepository.UnbanUser(userId));
        }
    }
}