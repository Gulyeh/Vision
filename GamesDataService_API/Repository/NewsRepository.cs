using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GamesDataService_API.DbContexts;
using GamesDataService_API.Dtos;
using GamesDataService_API.Entities;
using GamesDataService_API.Helpers;
using GamesDataService_API.Repository.IRepository;
using GamesDataService_API.Services.IServices;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GamesDataService_API.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly IUploadService uploadService;
        private readonly ICacheService cacheService;
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IMemoryCache memoryCache;

        public NewsRepository(IUploadService uploadService, ICacheService cacheService ,ApplicationDbContext db, IMapper mapper, IMemoryCache memoryCache)
        {
            this.uploadService = uploadService;
            this.cacheService = cacheService;
            this.db = db;
            this.mapper = mapper;
            this.memoryCache = memoryCache;
        }

        public async Task<ResponseDto> AddNews(AddNewsDto data)
        {
            var game = await db.Games.Include(x => x.News).FirstOrDefaultAsync(x => x.Id == data.GameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game not found" });

            var results = await uploadService.UploadPhoto(data.Photo);
            if(results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload image" });

            var mapped = mapper.Map<News>(data);
            mapped.PhotoId = results.PublicId;
            mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;

            game.News.Add(mapped);
            if(await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryRemoveFromCache<News>(CacheType.News);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Added news successfuly" });
            }
            
            await uploadService.DeletePhoto(results.PublicId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add news" });
        }

        public async Task<ResponseDto> DeleteNews(Guid newsId, Guid gameId)
        {
            var game = await db.Games.Include(x => x.News).FirstOrDefaultAsync(x => x.Id == gameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var news = game.News.FirstOrDefault(x => x.Id == newsId);
            if(news is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "News does not exist" });

            var results = await uploadService.DeletePhoto(news.PhotoId);
            if(results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete image" });

            game.News.Remove(news);
            if(await db.SaveChangesAsync() > 0){ 
                await cacheService.TryRemoveFromCache<News>(CacheType.News);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Deleted news successfuly" });
            }
            
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete news" });
        }

        public async Task<ResponseDto> EditNews(NewsDto data)
        {
            string oldPhotoId = string.Empty;
            var game = await db.Games.Include(x => x.News).FirstOrDefaultAsync(x => x.Id == data.GameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var news = game.News.FirstOrDefault(x => x.Id == data.Id);
            if(news is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "News does not exist" });

            if(data.Photo is not null){
                var results = await uploadService.UploadPhoto(data.Photo);
                if(results.Error is null) {
                    oldPhotoId = news.PhotoId;
                    news.PhotoId = results.PublicId;
                    news.PhotoUrl = results.SecureUrl.AbsoluteUri;
                }
            }

            mapper.Map(data, news);
            if(await db.SaveChangesAsync() > 0){
                if(!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Edited news successfuly" });
            } 
            
            await uploadService.DeletePhoto(news.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit news" });
        }

        public async Task<ResponseDto> GetGameNews(Guid gameId)
        {
            IEnumerable<News> news = await cacheService.TryGetFromCache<News>(CacheType.News);

            IEnumerable<News> gameNews = news.Where(x => x.GameId == gameId).OrderByDescending(x => x.Id).Take(5);
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<NewsDto>>(news));
        }
    }
}