namespace ProductsService_API.Services.IServices
{
    public interface IGameDataService
    {
        Task<T?> CheckGameExists<T>(Guid gameId, string Access_Token);
    }
}