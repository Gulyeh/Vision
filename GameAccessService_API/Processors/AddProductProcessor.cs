using GameAccessService_API.DbContexts;
using GameAccessService_API.Processors.Interfaces;
using GameAccessService_API.Services.IServices;

namespace GameAccessService_API.Processors
{
    public interface IAddProductProcessor
    {
        IProduct GenerateProduct(Guid gameId);
    }

    public class AddProductProcessor : IAddProductProcessor
    {
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;

        public AddProductProcessor(ICacheService cacheService, ApplicationDbContext db)
        {
            this.cacheService = cacheService;
            this.db = db;
        }

        public IProduct GenerateProduct(Guid gameId)
        {
            if (gameId == Guid.Empty) return new UserGamesData(cacheService, db);
            return new UserProductData(cacheService, db);
        }
    }
}