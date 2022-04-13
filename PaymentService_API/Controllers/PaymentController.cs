using Microsoft.AspNetCore.Mvc;
using PaymentService_API.Helpers;
using PaymentService_API.Repository.IRepository;

namespace PaymentService_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepository paymentRepository;

        public PaymentController(IPaymentRepository paymentRepository)
        {
            this.paymentRepository = paymentRepository;
        }

        [HttpGet("Success")]
        public async Task<ContentResult> PaymentSuccess([FromQuery] string sessionId)
        {
            await paymentRepository.PaymentCompleted(sessionId, PaymentStatus.Completed);
            return base.Content(@"
                    <body>
                        <script type='text/javascript'>
                            window.close();
                        </script>
                    </body>");
        }

        [HttpGet("Failed")]
        public async Task<ContentResult> PaymentFailed([FromQuery] string sessionId)
        {
            await paymentRepository.PaymentCompleted(sessionId, PaymentStatus.Cancelled);
            return base.Content(@"
                    <body>
                        <script type='text/javascript'>
                            window.close();
                        </script>
                    </body>");
        }
    }
}