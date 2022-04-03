using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Repository.IRepository;
using CodesService_API.Statics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodesService_API.Controllers
{
    public class CodesController : BaseApiController
    {
        private readonly ICodesRepository codesRepository;

        public CodesController(ICodesRepository codesRepository)
        {
            this.codesRepository = codesRepository;
        }

        [AllowAnonymous]
        [HttpGet("Validate")]
        public async Task<ActionResult<ResponseDto>> ValidateCode([FromQuery]string code){
            if(string.IsNullOrEmpty(code)) return BadRequest();
            return CheckActionResult(await codesRepository.CheckCode(code));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetCodes(){
            return Ok(await codesRepository.GetAllCodes());
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpDelete]
        public async Task<ActionResult<ResponseDto>> RemoveCode([FromQuery]int codeId){
            if(codeId <= 0) return BadRequest();
            return CheckActionResult(await codesRepository.RemoveCode(codeId));
        }

        [Authorize(Roles = StaticData.AdminRole)]
        [HttpPost("EditCode")]
        public async Task<ActionResult<ResponseDto>> EditCode([FromBody]CodesDataDto data){
            if(!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await codesRepository.EditCode(data));
        }
    }
}