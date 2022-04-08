using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService_API.Dtos;

namespace UsersService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseApiController : ControllerBase
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