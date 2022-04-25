using AutoMapper;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;
using GameAccessService_API.Helpers;
using GameAccessService_API.Processors;
using GameAccessService_API.Repository.IRepository;
using GameAccessService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace GameAccessService_API.Repository
{
    public class AccessRepository : IAccessRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly ILogger<AccessRepository> logger;

        public AccessRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService, ILogger<AccessRepository> logger)
        {
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<ResponseDto> BanUserAccess(UserAccessDto data)
        {
            var userAccess = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, data.UserId);
            var isBanned = userAccess.FirstOrDefault(x => x.GameId == data.GameId && x.ExpireDate > DateTime.UtcNow);
            if (isBanned is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "User is already banned on this game" });

            var mapped = mapper.Map<UserAccess>(data);
            var result = await db.UsersGameAccess.AddAsync(mapped);

            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<UserAccess>(CacheType.GameAccess, mapped);
                logger.LogInformation("User with ID: {userId} has been banned successfuly until: {date}", data.UserId, data.ExpireDate);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been banned successfuly" });
            }
            
            logger.LogError("Could not ban user with ID: {userId}", data.UserId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not ban a user" });
        }

        public async Task<bool> CheckUserAccess(Guid gameId, Guid userId)
        {
            return await checkCache<UserAccess>(gameId, userId, CacheType.GameAccess);
        }

        public async Task<bool> CheckUserHasGame(Guid gameId, Guid userId)
        {
            return await checkCache<UserGames>(gameId, userId, CacheType.OwnGame);
        }

        public async Task<bool> CheckUserHasProduct(Guid productId, Guid userId)
        {
            return await checkCache<UserProducts>(productId, userId, CacheType.OwnProduct);
        }

        public async Task<bool> AddProductOrGame(Guid userId, Guid gameId, Guid? productId = null)
        {
            var productData = new AddProductProcessor(cacheService, db).GenerateProduct(productId);
            productData.SetData(userId, gameId, productId);
            await productData.SaveData();

            if (await db.SaveChangesAsync() > 0)
            {
                await productData.AddToCache();
                logger.LogInformation("Granted Game/Product Access with ID: {productId} to User with ID: {userId}", 
                    productId == null ? gameId : productId, userId);
                return true;
            }

            logger.LogError("Could not grant access to Game/Product with ID: {productId} to User with ID: {userId}", 
                productId == null ? gameId : productId, userId);
            return false;
        }

        public async Task<ResponseDto> UnbanUserAccess(AccessDataDto data)
        {
            var userAccess = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, data.UserId);
            var isBanned = userAccess.FirstOrDefault(x => x.GameId == data.GameId);
            if (isBanned is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "User is not banned in this game" });

            db.UsersGameAccess.Remove(isBanned);
            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryRemoveFromCache<UserAccess>(CacheType.GameAccess, isBanned);
                logger.LogInformation("User with ID: {userId} has been unbanned successfuly", data.UserId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "User has been unbanned successfuly" });
            }

            logger.LogError("User with ID: {userId} could not be unbanned", data.UserId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not unban a user" });
        }

        private async Task<bool> checkCache<T>(Guid gameId, Guid userId, CacheType type) where T : BaseUser
        {
            var cache = await cacheService.TryGetFromCache<T>(type, userId);
            var results = cache.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);

            switch (type)
            {
                case CacheType.GameAccess:
                    if (results is null) {
                        logger.LogInformation("User with ID: {userId} has access to {gameId}", userId, gameId);
                        return true;
                    }
                    logger.LogInformation("User with ID: {userId} does not have access to {gameId}", userId, gameId);
                    return false;
                case CacheType.OwnGame:
                case CacheType.OwnProduct:
                    if (results is null) {
                        logger.LogInformation("User with ID: {userId} does not own {gameId}", userId, gameId);
                        return false;
                    }
                    logger.LogInformation("User with ID: {userId} own {gameId}", userId, gameId);
                    return true;
                default:
                    return false;
            }
        }
    }
}