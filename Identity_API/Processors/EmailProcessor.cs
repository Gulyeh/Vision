using Identity_API.DbContexts;
using Identity_API.Helpers;
using Identity_API.Processors.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity_API.Processors
{
    public interface IEmailProcessor
    {
        IEmail GenerateEmail(EmailTypes type, ApplicationUser user);
    }

    public class EmailProcessor : IEmailProcessor
    {
        private readonly UserManager<ApplicationUser> userManager;
        public EmailProcessor(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public IEmail GenerateEmail(EmailTypes type, ApplicationUser user)
        {
            return type switch
            {
                EmailTypes.Confirmation => new ConfirmEmail(userManager, user),
                EmailTypes.ResetPassword => new ResetPasswordEmail(userManager, user),
                _ => new ConfirmEmail(userManager, user)
            };
        }
    }
}