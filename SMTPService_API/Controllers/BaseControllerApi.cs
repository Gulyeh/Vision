using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMTPService_API.Dtos;

namespace SMTPService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseControllerApi : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ResponseDto> CheckActionResult(ResponseDto result){
            switch(result.Status){
                case 404:
                    return NotFound(result);
                case 400:
                    return BadRequest(result);
                case 204:
                    return NoContent();
                default:
                    return Ok(result);
            }
        }
    }
}