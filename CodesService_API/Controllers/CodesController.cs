using CodesService_API.Dtos;
using CodesService_API.Extensions;
using CodesService_API.Helpers;
using CodesService_API.Repository.IRepository;
using CodesService_API.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodesService_API.Controllers
{
    public class CodesController : BaseApiController
    {
        private readonly ICodesRepository codesRepository;
        private readonly IServiceScopeFactory scopeFactory;

        public CodesController(ICodesRepository codesRepository, IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
            this.codesRepository = codesRepository;
        }

        [HttpPut("ApplyCode")]
        public async Task<ActionResult<ResponseDto>> ApplyCode([FromQuery] string code, [FromQuery] string codeType)
        {
            if (string.IsNullOrEmpty(code)) return BadRequest();
            var token = HttpContext.Request.Headers["Authorization"][0];
            Enum.TryParse(codeType, true, out CodeTypes CodeType);
            return CheckActionResult(await codesRepository.ApplyCode(code, User.GetId(), CodeType, token));
        }

        [HttpGet("ValidateCode")]
        public async Task<ActionResult<ResponseDto>> ValidateCode([FromQuery] string code, [FromQuery] string codeType)
        {
            if (string.IsNullOrEmpty(code)) return BadRequest();
            Enum.TryParse(codeType, true, out CodeTypes CodeType);
            return CheckActionResult(await codesRepository.CheckCode(code, CodeType, User.GetId()));
        }


        [Authorize(Roles = StaticData.AdminRole)]
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetCodes()
        {
            return Ok(await codesRepository.GetAllCodes());
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpDelete]
        public async Task<ActionResult<ResponseDto>> RemoveCode([FromQuery] int codeId)
        {
            if (codeId <= 0) return BadRequest();
            return CheckActionResult(await codesRepository.RemoveCode(codeId));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPut("EditCode")]
        public async Task<ActionResult<ResponseDto>> EditCode([FromBody] CodesDataDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await codesRepository.EditCode(data));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPost("AddCode")]
        public async Task<ActionResult<ResponseDto>> AddCode([FromBody] AddCodesDto data)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return CheckActionResult(await codesRepository.AddCode(data));
        }
    }
}