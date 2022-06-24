using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService_API.Dtos;
using PaymentService_API.Helpers;
using PaymentService_API.Repository.IRepository;
using PaymentService_API.Statics;

namespace PaymentService_API.Controllers
{

    public class PaymentController : BaseApiController
    {
        private readonly IPaymentRepository paymentRepository;
        private readonly IValidateJWT validateJWT;

        public PaymentController(IPaymentRepository paymentRepository, IValidateJWT validateJWT)
        {
            this.paymentRepository = paymentRepository;
            this.validateJWT = validateJWT;
        }

        [HttpGet("GetPaymentMethods")]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> GetPaymentMethods()
        {
            return CheckActionResult(await paymentRepository.GetPaymentMethods());
        }

        [HttpPost("Success")]
        public async Task<ActionResult<ResponseDto>> PaymentSuccess([FromBody] PaymentCompletedDto data)
        {
            if (!validateJWT.IsTokenValid(data.Token, out string decodedToken)) return Unauthorized();
            return CheckActionResult(await paymentRepository.PaymentCompleted(data.SessionId, PaymentStatus.Completed, decodedToken));
        }

        [HttpPost("Failed")]
        public async Task<ActionResult<ResponseDto>> PaymentFailed([FromBody] PaymentCompletedDto data)
        {
            if (string.IsNullOrEmpty(data.SessionId)) return BadRequest();
            return CheckActionResult(await paymentRepository.PaymentCompleted(data.SessionId, PaymentStatus.Cancelled, string.Empty));
        }

        [HttpGet("GetNewProviders")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> GetNewProviders(){
            return new ResponseDto(true, StatusCodes.Status200OK, await paymentRepository.GetNewProviders());
        }

        [HttpPost("AddPaymentMethod")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> AddPaymentMethod([FromBody] AddPaymentMethodDto data)
        {
            if(!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await paymentRepository.AddPaymentMethod(data));
        }

        [HttpDelete("DeletePaymentMethod")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeletePaymentMethod([FromQuery] Guid paymentId)
        {
            if(paymentId == Guid.Empty) return BadRequest();
            return CheckActionResult(await paymentRepository.DeletePaymentMethod(paymentId));
        }

        [HttpPut("UpdatePaymentMethod")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> UpdatePaymentMethod([FromBody] EditPaymentMethodDto data)
        {
            if(!ModelState.IsValid) return BadRequest();
            return CheckActionResult(await paymentRepository.UpdatePaymentMethod(data));
        }
    }
}