using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace PaymentService_API.Helpers
{
    public interface IValidateJWT
    {
        bool IsTokenValid(string token, out string decodedToken);
        string EncodeToken(string token);
    }

    public class ValidateJWTHelper : IValidateJWT
    {
        private readonly IConfiguration config;

        public ValidateJWTHelper(IConfiguration config)
        {
            this.config = config;
        }

        public string EncodeToken(string token)
        {
            byte[] encrypted;
            Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(config.GetValue<string>("TokenHashKey"));
            aes.IV = Encoding.UTF8.GetBytes(config.GetValue<string>("TokenHashVector"));
            ICryptoTransform enc = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new())
            {
                using CryptoStream cs = new(ms, enc, CryptoStreamMode.Write);
                using (StreamWriter sw = new(cs))
                {
                    sw.Write(token);
                }

                encrypted = ms.ToArray();
            }

            return HttpUtility.UrlEncode(Convert.ToBase64String(encrypted));
        }

        private string DecodeToken(string token)
        {
            if (string.IsNullOrEmpty(token)) return string.Empty;

            string? decrypted = null;
            byte[] cipher = Convert.FromBase64String(token);
            var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(config.GetValue<string>("TokenHashKey"));
            aes.IV = Encoding.UTF8.GetBytes(config.GetValue<string>("TokenHashVector"));
            ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new(cipher))
            {
                using CryptoStream cs = new(ms, dec, CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                decrypted = sr.ReadToEnd();
            }
            return decrypted;
        }

        public bool IsTokenValid(string token, out string decodedToken)
        {
            try
            {
                token = DecodeToken(token);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetValue<string>("PrivateKey"))),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                decodedToken = token;
                return true;
            }
            catch (Exception)
            {
                decodedToken = string.Empty;
                return false;
            }
        }
    }
}