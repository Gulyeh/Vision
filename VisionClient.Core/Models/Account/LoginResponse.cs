using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Enums;

namespace VisionClient.Core.Models.Account
{
    public class LoginResponse
    {
        public LoginResponse()
        {
            Data = new object();
        }

        public LoginResponseTypes ResponseType { get; set; }
        public object Data { get; set; }
    }
}
