using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMTPService_API.Dtos;

namespace SMTPService_API.Services.IServices
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailDataDto data);
    }
}