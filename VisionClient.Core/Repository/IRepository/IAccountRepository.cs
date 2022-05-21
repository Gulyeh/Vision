using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Enums;
using VisionClient.Core.Models.Account;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IAccountRepository
    {
        Task<LoginResponse> LoginUser(string email, string password, string? AuthCode = null);
        Task<(bool, string?)> RegisterUser(string email, string password, string repeatpassword);
        Task<(bool, string?)> RequestPasswordReset(string email);
    }
}
