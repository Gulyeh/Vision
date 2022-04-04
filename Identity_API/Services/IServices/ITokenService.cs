using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.DbContexts;
using Microsoft.AspNetCore.Identity;

namespace Identity_API.Services.IServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user);
    }
}