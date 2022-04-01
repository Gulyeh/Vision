using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SMTPService_API.Dtos;
using SMTPService_API.Entities;
using SMTPService_API.Repository.IRepository;

namespace SMTPService_API.Controllers
{
    public class EmailController : BaseControllerApi
    {
        private readonly IEmailRepository emailRepository;

        public EmailController(IEmailRepository emailRepository)
        {
            this.emailRepository = emailRepository;
        }

        [HttpPost("SendEmailConfrimation")]
        public async Task<ActionResult<ResponseDto>> ConfirmEmail([FromBody]EmailDataDto data)
        {
            data.EmailType = EmailTypes.Confirmation;
            return CheckActionResult(await emailRepository.InitializeEmail(data));
        }

        [HttpPost("SendPaymentConfrimation")]
        public async Task<ActionResult<ResponseDto>> ConfirmPayment([FromBody]EmailDataDto data)
        {
            data.EmailType = EmailTypes.Payment;
            return CheckActionResult(await emailRepository.InitializeEmail(data));
        }

        [HttpPost("SendResetPassword")]
        public async Task<ActionResult<ResponseDto>> ResetPassword([FromBody]EmailDataDto data)
        {
            data.EmailType = EmailTypes.ResetPassword;
            return CheckActionResult(await emailRepository.InitializeEmail(data));
        }
    }
}