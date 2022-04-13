using Identity_API.DbContexts;

namespace Identity_API.Services.IServices
{
    public interface ITokenService
    {
        Task<string> CreateToken(ApplicationUser user);
    }
}