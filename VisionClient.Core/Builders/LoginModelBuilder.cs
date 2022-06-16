using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Builders
{
    internal class LoginModelBuilder
    {
        private LoginModel login = new LoginModel();

        public void SetEmail(string email)
        {
            login.Email = email;
        }

        public void SetPassword(string password)
        {
            login.Password = password;
        }

        public void SetAuthCode(string? AuthCode)
        {
            login.AuthCode = AuthCode;
        }

        public LoginModel Build()
        {
            return login;
        }
    }
}
