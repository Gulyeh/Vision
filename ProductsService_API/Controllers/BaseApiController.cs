using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductsService_API.Dtos;

namespace ProductsService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BaseApiController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult<ResponseDto> CheckActionResult(ResponseDto result)
        {
            switch (result.Status)
            {
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