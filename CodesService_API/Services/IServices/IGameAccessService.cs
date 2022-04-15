using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Services.IServices
{
    public interface IGameAccessService
    {
        Task<bool> CheckAccess(string? gameId, string Access_Token, string? productId = null);
    }
}