using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMTPService_API.Messages;

namespace SMTPService_API.Repository.IRepository
{
    public interface IEmailRepository
    {
        Task InitializeEmail(EmailDataDto data);
    }
}