using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Dtos;
using Identity_API.Entities;
using Identity_API.Repository.IRepository;
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

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseDto>> LoginUser(LoginDto data){
            if(!ModelState.IsValid) return BadRequest();

            var results = await accountRepository.Login(data);
            if(results.Status == 401){
                return Unauthorized(results);
            }
            return Ok(results);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult<ResponseDto>> Logout(){
            return Ok(await accountRepository.SingOut());
        }
        
        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<ResponseDto>> ConfirmEmail([FromQuery] ConfirmEmailQuery emailQuery){
            if(!ModelState.IsValid) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            
            var results = await accountRepository.ConfirmEmail(emailQuery.userId, emailQuery.token);
            if(results.Status == 400) return BadRequest(results);

            return Ok(results);
        }
    }
}