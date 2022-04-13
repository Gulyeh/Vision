using SMTPService_API.DBContexts;
using SMTPService_API.Entites;
using SMTPService_API.Messages;
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

        public async Task InitializeEmail(EmailDataDto data)
        {
            var sent = await emailSerivce.SendEmail(data);

            EmailLogs logs = new EmailLogs()
            {
                Email = data.ReceiverEmail,
                Log = $"Sent {data.EmailType} email"
            };

            await db.EmailLogs.AddAsync(logs);
            await db.SaveChangesAsync();
        }
    }
}