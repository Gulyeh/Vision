using Identity_API.Dtos;
using Identity_API.Helpers;
using Identity_API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_API.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseControllerApi
    {
        private readonly IAccountRepository accountRepository;
        private string BaseUri { get; init; }

        public AccountController(IAccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
            BaseUri = Url.Content("~/");
        }

        [HttpPost("Register")]
        public async Task<ActionResult<ResponseDto>> RegisterUser(RegisterDto data)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong model data" }));
            return CheckActionResult(await accountRepository.Register(data, BaseUri));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<ResponseDto>> LoginUser(LoginDto data)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong model data" }));
            return CheckActionResult(await accountRepository.Login(data));
        }

        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<ResponseDto>> ConfirmEmail([FromQuery] ConfirmEmailQuery emailQuery)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.ConfirmEmail(emailQuery.userId, emailQuery.token));
        }

        [HttpGet("RequestResetPassword")]
        public async Task<ActionResult<ResponseDto>> RequestResetPassword([FromQuery] string Email){
            if(string.IsNullOrEmpty(Email)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Please provide an email address" });
            return CheckActionResult(await accountRepository.RequestResetPassword(Email, BaseUri));
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResponseDto>> ResetPassword([FromBody] ResetPasswordData data){
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.ResetPassword(data));
        }
    }
}