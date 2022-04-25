using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.DbContexts;
using Identity_API.Helpers;
using Identity_API.Processors.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity_API.Processors
{
    public class EmailProcessor
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationUser user;

        public EmailProcessor(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            this.userManager = userManager;
            this.user = user;
        }

        public IEmail GenerateEmail(EmailTypes type){
            return type switch{
                EmailTypes.Confirmation => new ConfirmEmail(userManager, user),
                EmailTypes.ResetPassword => new ResetPasswordEmail(userManager, user),
                _ => new ConfirmEmail(userManager, user)
            };
        }
    }
}