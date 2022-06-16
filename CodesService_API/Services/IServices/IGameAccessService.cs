namespace CodesService_API.Services.IServices
{
    public interface IGameAccessService
    {
        Task<bool> CheckAccess(Guid gameId, string Access_Token, Guid productId);
    }
}