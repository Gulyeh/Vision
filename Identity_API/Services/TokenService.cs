using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HashidsNet;
using Identity_API.DbContexts;
using Identity_API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Identity_API.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config, UserManager<ApplicationUser> userManager)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["PrivateKey"]));
            this.userManager = userManager;
        }

        public async Task<string> CreateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString())
            };

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}