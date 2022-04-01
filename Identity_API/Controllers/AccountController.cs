using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Dtos;
using Identity_API.Entities;
using Identity_API.Extensions;
using Identity_API.Repository.IRepository;
using Identity_API.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_API.Controllers
{
    public class AccountController : BaseControllerApi
    {
        private readonly IAccountRepository accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult<ResponseDto>> RegisterUser(RegisterDto data){
            if(!ModelState.IsValid) return BadRequest();

            var baseUri = Url.Content("~/");
            var results = await accountRepository.Register(data, baseUri);
            if(results.Status == 400){
                return BadRequest(results);
            }
            return Ok(results);
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ResponseDto>> LoginUser(LoginDto data){
            if(!ModelState.IsValid) return BadRequest();

            var results = await accountRepository.Login(data);
            if(results.Status == 401){
                return Unauthorized(results);
            }
            return Ok(results);
        }

        [HttpGet("Logout")]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> Logout(){
            return Ok(await accountRepository.SingOut());
        }
        
        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<ResponseDto>> ConfirmEmail([FromQuery] ConfirmEmailQuery emailQuery){
            if(!ModelState.IsValid) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            
            var results = await accountRepository.ConfirmEmail(emailQuery.userId, emailQuery.token);
            if(results.Status == 400) return BadRequest(results);

            return Ok(results);
        }

        [HttpPost("BanUser")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> BanUserAccount([FromBody]BannedUsersDto data)
        {
            if(!ModelState.IsValid) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            if(data.UserId == User.GetId()) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot ban yourself" });
            return CheckActionResult(await accountRepository.BanUser(data));
        }

        [HttpPost("UnbanUser")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> UnbanUserAccount([FromQuery]string userId)
        {
            if(userId is null || string.IsNullOrEmpty(userId)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            return CheckActionResult(await accountRepository.UnbanUser(userId));
        }
    }
}