using AutoMapper;
using GameAccessService_API.DbContexts;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;
using GameAccessService_API.Helpers;
using GameAccessService_API.Processors;
using GameAccessService_API.Repository.IRepository;
using GameAccessService_API.Services.IServices;

namespace GameAccessService_API.Repository
{
    public class AccessRepository : IAccessRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly ILogger<AccessRepository> logger;
        private readonly IAddProductProcessor addProductProcessor;

        public AccessRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService, ILogger<AccessRepository> logger, IAddProductProcessor addProductProcessor)
        {
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.logger = logger;
            this.addProductProcessor = addProductProcessor;
        }

        public async Task<ResponseDto> BanUserAccess(UserAccessDto data)
        {
            var userAccess = await cacheService.TryGetFromCache<UserAccess>(CacheType.GameAccess, data.UserId);
            var isBanned = userAccess.FirstOrDefault(x => x.GameId == data.GameId && x.BanExpires > DateTime.UtcNow);
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

        public async Task<(bool, BanModelDto?)> CheckUserAccess(Guid gameId, Guid userId)
        {
            (var hasAccess, var bannedData) = await CheckAccessCacheAndGetData<UserAccess>(gameId, userId, CacheType.GameAccess);
            if (hasAccess) return (true, null);
            return (false, mapper.Map<BanModelDto>(bannedData));
        }


        public async Task<bool> CheckUserHasGame(Guid gameId, Guid userId) => await CheckAccessCache<UserGames>(gameId, userId, CacheType.OwnGame);


        public async Task<bool> CheckUserHasProduct(Guid productId, Guid gameId, Guid userId)
        {
            if (!await CheckUserHasGame(gameId, userId)) return true;
            return await CheckAccessCache<UserProducts>(productId, userId, CacheType.OwnProduct);
        }

        public async Task<bool> AddProductOrGame(Guid userId, Guid gameId, Guid productId)
        {
            var productData = addProductProcessor.GenerateProduct(gameId);
            productData.SetData(userId, gameId, productId);
            await productData.SaveData();

            if (await db.SaveChangesAsync() > 0)
            {
                await productData.AddToCache();
                logger.LogInformation("Granted Game/Product Access with ID: {productId} to User with ID: {userId}",
                    productId == Guid.Empty ? gameId : productId, userId);
                return true;
            }

            logger.LogError("Could not grant access to Game/Product with ID: {productId} to User with ID: {userId}",
                productId == Guid.Empty ? gameId : productId, userId);
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

        private async Task<(bool, T?)> CheckAccessCacheAndGetData<T>(Guid Id, Guid userId, CacheType type) where T : BaseUser
        {
            var cache = await cacheService.TryGetFromCache<T>(type, userId);
            switch (type)
            {
                case CacheType.GameAccess:

                    var productsAccess = cache as IEnumerable<UserAccess>;
                    if (productsAccess is null) return (false, null);

                    var access = productsAccess.FirstOrDefault(x => x.GameId == Id && x.BanExpires > DateTime.UtcNow);
                    if (access is null)
                    {
                        logger.LogInformation("User with ID: {userId} has access to {gameId}", userId, Id);
                        return (true, null);
                    }

                    logger.LogInformation("User with ID: {userId} does not have access to {gameId}", userId, Id);
                    return (false, access as T);
                default:
                    return (false, null);
            }
        }

        private async Task<bool> CheckAccessCache<T>(Guid Id, Guid userId, CacheType type) where T : BaseUser
        {
            var cache = await cacheService.TryGetFromCache<T>(type, userId);

            switch (type)
            {
                case CacheType.GameAccess:
                    var productsAccess = cache as IEnumerable<UserAccess>;
                    if (productsAccess is null) return false;

                    var access = productsAccess.FirstOrDefault(x => x.GameId == Id);
                    if (access is null)
                    {
                        logger.LogInformation("User with ID: {userId} has access to {gameId}", userId, Id);
                        return true;
                    }

                    logger.LogInformation("User with ID: {userId} does not have access to {gameId}", userId, Id);
                    return false;
                case CacheType.OwnGame:
                    var ownGames = cache as IEnumerable<UserGames>;
                    if (ownGames is null) return false;

                    var ownResults = ownGames.FirstOrDefault(x => x.GameId == Id);
                    if (ownResults is null)
                    {
                        logger.LogInformation("User with ID: {userId} does not own Game: {Id}", userId, Id);
                        return false;
                    }

                    logger.LogInformation("User with ID: {userId} own Game: {Id}", userId, Id);
                    return true;
                case CacheType.OwnProduct:
                    var products = cache as IEnumerable<UserProducts>;
                    if (products is null) return false;

                    var productResult = products.FirstOrDefault(x => x.ProductId == Id);
                    if (productResult is null)
                    {
                        logger.LogInformation("User with ID: {userId} does not own Product: {Id}", userId, Id);
                        return false;
                    }

                    logger.LogInformation("User with ID: {userId} own Product: {Id}", userId, Id);
                    return true;
                default:
                    return false;
            }
        }
    }
}