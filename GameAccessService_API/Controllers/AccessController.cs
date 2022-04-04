using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;
using GameAccessService_API.Extensions;
using GameAccessService_API.Helpers;
using GameAccessService_API.Repository.IRepository;
using GameAccessService_API.Statics;
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
        public async Task<ActionResult<ResponseDto>> CheckAccess([FromQuery]Guid gameId){
            if(gameId == Guid.Empty) return BadRequest();
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, new HasAccess(await accessRepository.CheckUserAccess(gameId, User.GetId()))));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPost("BanUser")]
        public async Task<ActionResult<ResponseDto>> BanUser([FromBody]UserAccessDto data){
            if(!ModelState.IsValid) return BadRequest(ModelState);
            if(User?.GetId() == data.UserId) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot ban yourself" }));
            return CheckActionResult(await accessRepository.BanUserAccess(data));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPost("UnbanUser")]
        public async Task<ActionResult<ResponseDto>> UnbanUser([FromBody]AccessDataDto data){
            if(!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await accessRepository.UnbanUserAccess(data));
        }
    }
}