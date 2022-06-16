using Microsoft.AspNetCore.Mvc;
using PaymentService_API.Dtos;

namespace PaymentService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : Controller
    {
        public BaseApiController()
        {
        }

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