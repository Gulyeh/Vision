using CodesService_API.Dtos;
using CodesService_API.Extensions;
using CodesService_API.Helpers;
using CodesService_API.Repository.IRepository;
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
            Enum.TryParse(codeType, true, out CodeTypes CodeType);
            return CheckActionResult(await codesRepository.ApplyCode(code, User.GetId(), CodeType));
        }

        [HttpGet("ValidateCode")]
        public async Task<ActionResult<ResponseDto>> ValidateCode([FromQuery] string code, [FromQuery] string codeType)
        {
            if (string.IsNullOrEmpty(code)) return BadRequest();
            Enum.TryParse(codeType, true, out CodeTypes CodeType);
            return CheckActionResult(await codesRepository.CheckCode(code, CodeType, User.GetId()));
        }


        [Authorize(Policy = "HasAdminRole")]
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetCodes()
        {
            return Ok(await codesRepository.GetAllCodes());
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpDelete]
        public async Task<ActionResult<ResponseDto>> RemoveCode([FromQuery] string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return BadRequest();
            return CheckActionResult(await codesRepository.RemoveCode(code));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPut("EditCode")]
        public async Task<ActionResult<ResponseDto>> EditCode([FromBody] EditCodeDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await codesRepository.EditCode(data));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpPost("AddCode")]
        public async Task<ActionResult<ResponseDto>> AddCode([FromBody] AddCodesDto data)
        {
            if (!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await codesRepository.AddCode(data));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpGet("GetUserUsedCodes")]
        public async Task<ActionResult<ResponseDto>> GetUserUsedCodes([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest();
            return CheckActionResult(await codesRepository.GetUserUsedCodes(userId));
        }

        [Authorize(Policy = "HasAdminRole")]
        [HttpDelete("DeleteUsedCode")]
        public async Task<ActionResult<ResponseDto>> DeleteUsedCode([FromQuery] Guid codeId)
        {
            if (codeId == Guid.Empty) return BadRequest();
            return CheckActionResult(await codesRepository.RemoveUsedCode(codeId));
        }
    }
}