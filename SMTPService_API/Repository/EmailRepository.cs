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
        private readonly ILogger<EmailRepository> logger;

        public EmailRepository(ApplicationDbContext db, IEmailService emailSerivce, ILogger<EmailRepository> logger)
        {
            this.db = db;
            this.emailSerivce = emailSerivce;
            this.logger = logger;
        }

        public async Task InitializeEmail(EmailDataDto data)
        {
            var sent = await emailSerivce.SendEmail(data);
            if (sent == true)
            {
                EmailLogs logs = new EmailLogs()
                {
                    Email = data.ReceiverEmail,
                    Log = $"Sent {data.EmailType} email"
                };

                await db.EmailLogs.AddAsync(logs);
                await db.SaveChangesAsync();
                logger.LogInformation("Sent message to User with ID: {Id} and Email: {email}", data.UserId, data.ReceiverEmail);
            }
        }
    }
}