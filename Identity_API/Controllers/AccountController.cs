using Identity_API.Dtos;
using Identity_API.Extensions;
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
        [HttpPut("Register")]
        public async Task<ActionResult<ResponseDto>> RegisterUser(RegisterDto data)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong model data" }));
            return CheckActionResult(await accountRepository.Register(data));
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<ResponseDto>> LoginUser(LoginDto data)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong model data" }));
            return CheckActionResult(await accountRepository.Login(data));
        }

        [AllowAnonymous]
        [HttpPost("ConfirmEmail")]
        public async Task<ActionResult<ResponseDto>> ConfirmEmail([FromBody] ConfirmEmailDto emailQuery)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.ConfirmEmail(emailQuery.UserId, emailQuery.Token));
        }

        [AllowAnonymous]
        [HttpGet("RequestResetPassword")]
        public async Task<ActionResult<ResponseDto>> RequestResetPassword([FromQuery] string Email)
        {
            if (string.IsNullOrEmpty(Email)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Please provide an email address" });
            return CheckActionResult(await accountRepository.RequestResetPassword(Email));
        }

        [AllowAnonymous]
        [HttpGet("ResendConfirmationEmail")]
        public async Task<ActionResult<ResponseDto>> ResendConfirmationEmail([FromQuery] string Email)
        {
            if (string.IsNullOrEmpty(Email)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Please provide an email address" });
            return CheckActionResult(await accountRepository.ResendEmailConfirmation(Email));
        }

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<ActionResult<ResponseDto>> ResetPassword([FromBody] ResetPasswordDto data)
        {
            if (!ModelState.IsValid) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.ResetPassword(data));
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ResponseDto>> ChangePassword([FromBody] PasswordDataDto data)
        {
            data.userId = HttpContext.User.GetId();
            if (!ModelState.IsValid || data.userId == Guid.Empty) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.ChangePassword(data));
        }

        [Authorize]
        [HttpGet("Generate2FA")]
        public async Task<ActionResult<ResponseDto>> Generate2FA()
        {
            var userId = HttpContext.User.GetId();
            if (userId == Guid.Empty) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.Generate2FA(userId));
        }

        [Authorize]
        [HttpPost("Toggle2FA")]
        public async Task<ActionResult<ResponseDto>> Toggle2FA([FromQuery] string code)
        {
            var userId = HttpContext.User.GetId();
            if (userId == Guid.Empty || string.IsNullOrEmpty(code)) return BadRequest(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Wrong data" }));
            return CheckActionResult(await accountRepository.Toggle2FA(userId, code));
        }
    }
}