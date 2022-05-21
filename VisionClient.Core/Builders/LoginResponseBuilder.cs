using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Enums;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Builders
{
    internal class LoginResponseBuilder
    {
        LoginResponse response = new LoginResponse();

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
