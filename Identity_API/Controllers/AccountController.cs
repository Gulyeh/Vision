using Identity_API.Dtos;
using Identity_API.Helpers;
using Identity_API.Repository.IRepository;
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
        public async Task<ActionResult<ResponseDto>> RegisterUser(RegisterDto data)
        {
            if (!ModelState.IsValid) return BadRequest();

            var baseUri = Url.Content("~/");
            return CheckActionResult(await accountRepository.Register(data, baseUri));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ResponseDto>> LoginUser(LoginDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await accountRepository.Login(data));
        }

        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<ResponseDto>> ConfirmEmail([FromQuery] ConfirmEmailQuery emailQuery)
        {
            if (!ModelState.IsValid) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" });
            return CheckActionResult(await accountRepository.ConfirmEmail(emailQuery.userId, emailQuery.token));
        }
    }
}