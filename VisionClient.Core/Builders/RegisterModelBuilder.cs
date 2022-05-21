using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Builders
{
    internal class RegisterModelBuilder
    {
        RegisterModel model = new RegisterModel();

        public void SetEmail(string email)
        {
            model.Email = email;
        }

        public void SetPassword(string password)
        {
            model.Password = password;
        }

        public void SetRepeatPassword(string repeatPassword)
        {
            model.ConfirmPassword = repeatPassword;
        }

        public RegisterModel Build()
        {
            return model;
        }
    }
}
