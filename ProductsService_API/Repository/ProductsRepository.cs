using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProdcutsService_API.RabbitMQSender;
using ProductsService_API.DbContexts;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Helpers;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IUploadService uploadService;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ICacheService cacheService;
        private readonly IGameDataService gameDataService;
        private readonly ILogger<ProductsRepository> logger;
        private readonly IGetCachedGames getCachedGames;
        private readonly IGameAccessService gameAccessService;

        public ProductsRepository(ApplicationDbContext db, IMapper mapper, IUploadService uploadService, IRabbitMQSender rabbitMQSender,
            ICacheService cacheService, IGameDataService gameDataService, ILogger<ProductsRepository> logger, IGetCachedGames getCachedGames, IGameAccessService gameAccessService)
        {
            this.getCachedGames = getCachedGames;
            this.gameAccessService = gameAccessService;
            this.db = db;
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.rabbitMQSender = rabbitMQSender;
            this.cacheService = cacheService;
            this.gameDataService = gameDataService;
            this.logger = logger;
        }

        public async Task<ResponseDto> AddProduct(AddProductsDto data, string Access_Token)
        {
            var checkGame = await gameDataService.CheckGameExists(data.GameId, Access_Token);
            if (!checkGame.isSuccess) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var mapped = mapper.Map<Products>(data);

            var results = await uploadService.UploadPhoto(data.Photo);
            if (results.Error is null)
            {
                mapped.PhotoId = results.PublicId;
                mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;
            }
            
            if(!string.IsNullOrWhiteSpace(mapped.PhotoUrl)){
                game.Products?.Add(mapped);
                if (await SaveChangesAsync())
                {                
                    await ReplaceGameProduct(data.GameId, game);                  
                    logger.LogInformation("Added Product to Game with ID: {gameId} for purchase successfully", data.GameId);
                    return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been added successfuly" });
                }
            }

            await uploadService.DeletePhoto(mapped.PhotoId);
            logger.LogError("Could not add Product to Game with ID: {gameId}", data.GameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add product" });
        }

        public async Task<ResponseDto> DeleteProduct(Guid productId, Guid gameId)
        {
            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == gameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            var product = game.Products.FirstOrDefault(x => x.Id == productId);
            if (product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });

            game.Products.Remove(product);

            if (await SaveChangesAsync())
            {
                await uploadService.DeletePhoto(product.PhotoId);
                await ReplaceGameProduct(gameId, game);
                rabbitMQSender.SendMessage(productId, "DeleteProductAccessQueue");

                logger.LogInformation("Deleted Product with ID: {productId} from Game with ID: {gameId} successfully", productId, gameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been removed successfuly" });
            }

            logger.LogError("Could not delete Product with ID: {productId} from Game with ID: {gameId}", productId, gameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not remove product" });
        }

        public async Task<ResponseDto> EditProduct(EditPackageDto data)
        {
            var oldPhotoId = string.Empty;

            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            var product = game.Products.FirstOrDefault(x => x.Id == data.Id);
            if (product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });

            mapper.Map(data, product);

            if (!string.IsNullOrWhiteSpace(data.Photo))
            {
                var results = await uploadService.UploadPhoto(data.Photo);
                if (results.Error is null)
                {
                    oldPhotoId = product.PhotoId;
                    product.PhotoId = results.PublicId;
                    product.PhotoUrl = results.SecureUrl.AbsoluteUri;
                }
            }

            if (await SaveChangesAsync())
            {
                if (!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);

                var newGame = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == data.GameId);
                if(newGame is not null) await ReplaceGameProduct(data.GameId, newGame);

                logger.LogInformation("Edited Product with ID: {productId} in Game with ID: {gameId} successfully", data.Id, data.GameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been edited successfuly" });
            }

            logger.LogError("Could not edit Product with ID: {productId} in Game with ID: {gameId}", data.Id, data.GameId);
            if (!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(product.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit product" });
        }

        public async Task<ResponseDto> GetProduct(Guid gameId, Guid productId, string Access_Token)
        {
            var getGamesCache = await getCachedGames.GetGames();
            var game = getGamesCache.FirstOrDefault(x => x.GameId == gameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            if (game.Products.Any())
            {
                var product = game.Products.FirstOrDefault(x => x.Id == productId);

                var response = await gameAccessService.CheckProductAccess(productId, gameId, Access_Token);
                if (response is null) return new ResponseDto(false, StatusCodes.Status500InternalServerError, string.Empty);

                var mapped = mapper.Map<ProductsDto>(product);

                var responseString = response.Response.ToString();
                if (string.IsNullOrEmpty(responseString)) return new ResponseDto(false, StatusCodes.Status500InternalServerError, string.Empty);
                else
                {
                    var json = JsonConvert.DeserializeObject<HasAccess>(responseString);
                    if (json is null) mapped.IsPurchased = false;
                    else mapped.IsPurchased = json.hasAccess;
                }

                return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<ProductsDto>(product));
            }

            return new ResponseDto(false, StatusCodes.Status400BadRequest, new ProductsDto());
        }

        public async Task<ResponseDto> ProductExists(Guid gameId, Guid productId)
        {
            var game = await db.Games.Include(x => x.Products).FirstOrDefaultAsync(x => x.GameId == gameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new { productExists = false });
            var product = game.Products?.FirstOrDefault(x => x.Id == productId);
            if (product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new { productExists = false });
            return new ResponseDto(true, StatusCodes.Status200OK, new { productExists = true });
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }

        private async Task ReplaceGameProduct(Guid gameId, Games Replacement){
            var cachedGame = await getCachedGames.GetGames();
            if(cachedGame is not null){
                var gameFound = cachedGame.FirstOrDefault(x => x.GameId == gameId);
                if(gameFound is not null) await cacheService.TryReplaceCache<Games>(CacheType.Games, gameFound, Replacement);
            }
        }
    }
}