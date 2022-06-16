using VisionClient.Core.Enums;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Builders
{
    internal class LoginResponseBuilder
    {
        readonly LoginResponse response = new();

        public void SetType(LoginResponseTypes type)
        {
            response.ResponseType = type;
        }

        public void SetData(object data)
        {
            response.Data = data;
        }

        public LoginResponse Build()
        {
            return response;
        }
    }
}
