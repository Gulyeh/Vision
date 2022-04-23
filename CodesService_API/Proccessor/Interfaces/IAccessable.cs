using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Services.IServices;

namespace CodesService_API.Proccessor.Interfaces
{
    public interface IAccessable : IResponder
    {
        Task<bool> CheckAccess(string? gameId, string Access_Token, string? productId = null);
    }
}