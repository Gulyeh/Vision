using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Enums
{
    public enum LoginResponseTypes
    {
        WrongCredentials,
        UserBanned,
        TwoFactorAuth,
        WrongAuthCode,
        Success
    }
}
