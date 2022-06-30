namespace GameAccessService_API.Processors.Interfaces
{
    public interface IProduct
    {
        void SetData(Guid userId, Guid gameId, Guid productId);
        Task<bool> OwnsProduct();
        Task AddToCache();
        Task SaveData();
    }
}