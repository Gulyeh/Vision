namespace VisionClient.Core.Models.Account
{
    public class LoginModel
    {
        public LoginModel()
        {
            Email = string.Empty;
            Password = string.Empty;
        }

        public string Email { get; set; }
        public string Password { get; set; }
        public string? AuthCode { get; set; }
    }
}
