using AutoMapper;
using GamesDataService_API.Builder;
using GamesDataService_API.DbContexts;
using GamesDataService_API.Dtos;
using GamesDataService_API.Entities;
using GamesDataService_API.Helpers;
using GamesDataService_API.Messages;
using GamesDataService_API.RabbitMQSender;
using GamesDataService_API.Repository.IRepository;
using GamesDataService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GamesDataService_API.Repository
{
    public class GamesRepository : IGamesRepository
    {
        private readonly IUploadService uploadService;
        private readonly ILogger<GamesRepository> logger;
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;
        private readonly IRabbitMQSender rabbitMQSender;

        public GamesRepository(IUploadService uploadService, ILogger<GamesRepository> logger,
            ICacheService cacheService, ApplicationDbContext db, IMapper mapper, IMemoryCache memoryCache, IRabbitMQSender rabbitMQSender)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.uploadService = uploadService;
            this.logger = logger;
            this.cacheService = cacheService;
            this.db = db;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        public async Task<ResponseDto> AddGame(AddGamesDto data)
        {
            var mapped = mapper.Map<Games>(data);

            var results = await uploadService.UploadPhoto(Convert.FromBase64String(data.CoverPhoto));
            mapped.CoverId = results.PublicId;
            mapped.CoverUrl = results.SecureUrl.AbsoluteUri;

            results = await uploadService.UploadPhoto(Convert.FromBase64String(data.IconPhoto));
            mapped.IconUrl = results.SecureUrl.AbsoluteUri;
            mapped.IconId = results.PublicId;

            results = await uploadService.UploadPhoto(Convert.FromBase64String(data.BannerPhoto));
            mapped.BannerUrl = results.SecureUrl.AbsoluteUri;
            mapped.BannerId = results.PublicId;


            if(!string.IsNullOrEmpty(mapped.IconId) && !string.IsNullOrEmpty(mapped.CoverId) && !string.IsNullOrEmpty(mapped.BannerId)){
                await db.Games.AddAsync(mapped);
                if (await db.SaveChangesAsync() > 0)
                {
                    if(mapped.IsAvailable) await cacheService.TryAddToCache<Games>(CacheType.Games, mapped);
                    
                    var builder = new ProductBuilder();
                    builder.SetTitle(mapped.Name);
                    builder.SetGameId(mapped.Id);
                    builder.SetPhotoUrl(mapped.CoverUrl);
                    builder.SetPhotoId(mapped.CoverId);
                    builder.SetPrice(data.Price);
                    builder.SetIsAvailable(data.IsPurchasable);
                    builder.SetDiscount(data.Discount);
                    builder.SetDetails(data.Details);

                    rabbitMQSender.SendMessage(builder.Build(), "CreateNewGameProductQueue");
                    logger.LogInformation("Added new game successfully");
                    return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Added game successfully" });
                }
            }

            if (!string.IsNullOrEmpty(mapped.IconId)) await uploadService.DeletePhoto(mapped.IconId);
            if (!string.IsNullOrEmpty(mapped.CoverId)) await uploadService.DeletePhoto(mapped.CoverId);
            if (!string.IsNullOrEmpty(mapped.BannerId)) await uploadService.DeletePhoto(mapped.BannerId);
            logger.LogError("Could not add game");
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add game" });
        }

        public async Task<ResponseDto> CheckGame(Guid gameId)
        {
            var gameList = await cacheService.TryGetFromCache<Games>(CacheType.Games);
            var game = gameList.FirstOrDefault(x => x.Id == gameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<GamesDto>(game));
        }

        public async Task<ResponseDto> DeleteGame(Guid gameId)
        {
            var game = await GameExists(gameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exists" });

            db.Games.Remove(game);
            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryRemoveFromCache<Games>(CacheType.Games, game);
                if (!string.IsNullOrEmpty(game.IconId)) await uploadService.DeletePhoto(game.IconId);
                if (!string.IsNullOrEmpty(game.CoverId)) await uploadService.DeletePhoto(game.CoverId);
                if (!string.IsNullOrEmpty(game.BannerId)) await uploadService.DeletePhoto(game.BannerId);

                rabbitMQSender.SendMessage(gameId, "DeleteGameProductQueue");
                logger.LogInformation("Deleted game with ID: {id} successfully", gameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Deleted game successfully" });
            }

            logger.LogError("Could not delete game with ID: {id}", gameId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete game" });
        }

        public async Task<ResponseDto> EditGameData(GamesDto data)
        {
            var oldCoverId = string.Empty;
            var oldIconId = string.Empty;
            var oldBannerId = string.Empty;

            var game = await GameExists(data.Id);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exists" });

            mapper.Map(data, game);

            if (!string.IsNullOrWhiteSpace(data.CoverPhoto))
            {
                var results = await uploadService.UploadPhoto(Convert.FromBase64String(data.CoverPhoto));
                oldCoverId = game.CoverId;
                game.CoverId = results.PublicId;
                game.CoverUrl = results.SecureUrl.AbsoluteUri;
            }

            if (!string.IsNullOrWhiteSpace(data.IconPhoto))
            {
                var results = await uploadService.UploadPhoto(Convert.FromBase64String(data.IconPhoto));
                oldIconId = game.IconId;
                game.IconId = results.PublicId;
                game.IconUrl = results.SecureUrl.AbsoluteUri;
            }

            if (!string.IsNullOrWhiteSpace(data.BannerPhoto))
            {
                var results = await uploadService.UploadPhoto(Convert.FromBase64String(data.BannerPhoto));
                oldBannerId = game.BannerId;
                game.BannerId = results.PublicId;
                game.BannerUrl = results.SecureUrl.AbsoluteUri;
            }

            if(!string.IsNullOrEmpty(game.BannerId) && !string.IsNullOrEmpty(game.IconId) && !string.IsNullOrEmpty(game.CoverId))
            {
                db.Games.Update(game);
                if (await db.SaveChangesAsync() > 0)
                {
                    if(!string.IsNullOrWhiteSpace(oldCoverId)) rabbitMQSender.SendMessage(new PhotoData(game.CoverUrl, game.CoverId, game.Id), "EditGameProductPhotoQueue");
                    logger.LogInformation("Edited game with ID: {id} successfully", data.Id);
                    return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game edited successfully" });
                }
            }
            
            if (!string.IsNullOrEmpty(oldIconId)) await uploadService.DeletePhoto(game.IconId);
            if (!string.IsNullOrEmpty(oldCoverId)) await uploadService.DeletePhoto(game.CoverId);
            if (!string.IsNullOrEmpty(oldBannerId)) await uploadService.DeletePhoto(game.BannerId);
            logger.LogError("Could not edit game with ID: {id}", data.Id);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit game" });
        }

        public async Task<ResponseDto> GetGames()
        {
            IEnumerable<Games> games = await cacheService.TryGetFromCache<Games>(CacheType.Games);
            if (games.Count() == 0)
            {
                var dbGames = await db.Games.Include(x => x.Requirements).Include(x => x.Informations).Where(x => x.IsAvailable).ToListAsync();
                foreach (var game in dbGames) await cacheService.TryAddToCache<Games>(CacheType.Games, game);
                games = dbGames;
            }
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<GamesDto>>(games));
        }

        private async Task<Games?> GameExists(Guid gameId)
        {
            IEnumerable<Games> gameExists = await cacheService.TryGetFromCache<Games>(CacheType.Games);
            if (gameExists is not null)
            {
                var findGame = gameExists.FirstOrDefault(x => x.Id == gameId);
                if (findGame is not null)
                {
                    return findGame;
                }
            }
            logger.LogError("Game with ID: {id} does not exist", gameId);
            return null;
        }
    }
}