using System.IdentityModel.Tokens.Jwt;

namespace VisionClient.Core.Helpers
{
    public static class GetJWTRoleHelper
    {
        public static string GetRole(string jwt_token)
        {
            var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt_token);
            var claim = token.Claims.First(c => c.Type == "role").Value;
            return string.IsNullOrEmpty(claim) ? "User" : claim;
        }
    }
}
