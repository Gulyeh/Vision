using AutoMapper;
using GamesDataService_API.DbContexts;
using GamesDataService_API.Dtos;
using GamesDataService_API.Entities;
using GamesDataService_API.Helpers;
using GamesDataService_API.Repository.IRepository;
using GamesDataService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GamesDataService_API.Repository
{
    public class GamesRepository : IGamesRepository
    {
        private readonly IUploadService uploadService;
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public GamesRepository(IUploadService uploadService, ICacheService cacheService, ApplicationDbContext db, IMapper mapper, IMemoryCache memoryCache)
        {
            this.uploadService = uploadService;
            this.cacheService = cacheService;
            this.db = db;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        public async Task<ResponseDto> AddGame(AddGamesDto data)
        {
            var mapped = mapper.Map<Games>(data);

            var results = await uploadService.UploadPhoto(data.CoverPhoto);
            if (results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload cover image" });
            mapped.CoverId = results.PublicId;
            mapped.CoverUrl = results.SecureUrl.AbsoluteUri;

            results = await uploadService.UploadPhoto(data.IconPhoto);
            if (results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload icon image" });
            mapped.IconUrl = results.SecureUrl.AbsoluteUri;
            mapped.IconId = results.PublicId;

            await db.Games.AddAsync(mapped);
            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<Games>(CacheType.Games, mapped);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Added game successfuly" });
            }

            if (!string.IsNullOrEmpty(mapped.IconId)) await uploadService.DeletePhoto(mapped.IconId);
            if (!string.IsNullOrEmpty(mapped.CoverId)) await uploadService.DeletePhoto(mapped.CoverId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add game" });
        }

        public async Task<ResponseDto> CheckGame(Guid gameId)
        {
            var game = await db.Games.FirstOrDefaultAsync(x => x.Id == gameId);

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
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Deleted game successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete game" });
        }

        public async Task<ResponseDto> EditGameData(GamesDto data)
        {
            var oldCoverId = string.Empty;
            var oldIconId = string.Empty;

            var game = await GameExists(data.Id);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exists" });

            mapper.Map(data, game);
            if (data.CoverPhoto is not null)
            {
                var results = await uploadService.UploadPhoto(data.CoverPhoto);
                if (results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload cover photo" });

                oldCoverId = game.CoverId;
                game.CoverId = results.PublicId;
                game.CoverUrl = results.SecureUrl.AbsoluteUri;
            }

            if (data.IconPhoto is not null)
            {
                var results = await uploadService.UploadPhoto(data.IconPhoto);
                if (results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload icon photo" });

                oldIconId = game.IconId;
                game.IconId = results.PublicId;
                game.IconUrl = results.SecureUrl.AbsoluteUri;
            }

            if (await db.SaveChangesAsync() > 0)
            {
                if (!string.IsNullOrEmpty(oldIconId)) await uploadService.DeletePhoto(oldIconId);
                if (!string.IsNullOrEmpty(oldCoverId)) await uploadService.DeletePhoto(oldCoverId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Game edited successfuly" });
            }

            if (!string.IsNullOrEmpty(oldIconId)) await uploadService.DeletePhoto(game.IconId);
            if (!string.IsNullOrEmpty(oldCoverId)) await uploadService.DeletePhoto(game.CoverId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit game" });
        }

        public async Task<ResponseDto> GetGames()
        {
            IEnumerable<Games> games = await cacheService.TryGetFromCache<Games>(CacheType.Games);
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<GamesDto>>(games));
        }

        private async Task<Games?> GameExists(Guid gameId)
        {
            IEnumerable<Games> gameExists = await cacheService.TryGetFromCache<Games>(CacheType.Games);
            if(gameExists is not null){
                var findGame = gameExists.FirstOrDefault(x => x.Id == gameId);
                if (findGame is not null)
                {
                    return findGame;
                }
            }       
            return null;
        }
    }
}