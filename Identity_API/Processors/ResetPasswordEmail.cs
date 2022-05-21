using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Helpers;
using Identity_API.Processors.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Identity_API.Processors
{
    public class ResetPasswordEmail : IEmail
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationUser user;
        private EmailDataDto email = new();

        public ResetPasswordEmail(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            this.userManager = userManager;
            this.user = user;
        }

        private async Task<string> GenerateToken(string baseUri){
            var tokenBytes = Encoding.UTF8.GetBytes(await userManager.GeneratePasswordResetTokenAsync(user));
            var token = HttpUtility.UrlEncode(Encoding.UTF8.GetString(tokenBytes));
            return $"{baseUri}/ResetPassword?userId={user.Id}&token={token}";
        }

        public EmailDataDto Build()
        {
            return email;
        }

        public async Task GenerateEmailData(string baseUri)
        {
            var link = await GenerateToken(baseUri);
            email.Content = $"Reset Password by clicking {link}";
            email.EmailType = EmailTypes.ResetPassword;
            email.ReceiverEmail = user.UserName;
            email.userId = user.Id;
        }
    }
}