using System.Security.Claims;

namespace PaymentService_API.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            var name = user?.FindFirst(ClaimTypes.Name)?.Value;
            return name is not null ? name : string.Empty;
        }

        public static Guid GetId(this ClaimsPrincipal user)
        {
            Guid id = Guid.Empty;
            Guid.TryParse(user?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out id);
            return id != Guid.Empty ? id : Guid.Empty;
        }
    }
}