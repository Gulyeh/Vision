using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMTPService_API.DBContexts;
using SMTPService_API.Dtos;
using SMTPService_API.Entities;
using SMTPService_API.Repository.IRepository;
using SMTPService_API.Services.IServices;

namespace SMTPService_API.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IEmailService emailSerivce;

        public EmailRepository(ApplicationDbContext db, IEmailService emailSerivce)
        {
            this.db = db;
            this.emailSerivce = emailSerivce;
        }

        public async Task<ResponseDto> InitializeEmail(EmailDataDto data)
        {
            var sent = await emailSerivce.SendEmail(data);

            if(!sent) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not send confirmation email" });

            EmailLogs logs = new EmailLogs(){
                Email = data.ReceiverEmail,
                Log = $"Sent {data.EmailType} email"
            };

            await db.EmailLogs.AddAsync(logs);
            if(await db.SaveChangesAsync() < 1) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not save email logs" });

            return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Email has been sent and logged" });
        }
    }
}