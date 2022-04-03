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

namespace GamesDataService_API.Repository
{
    public class NewsRepository : INewsRepository
    {
        private readonly IUploadService uploadService;
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;

        public NewsRepository(IUploadService uploadService, ApplicationDbContext db, IMapper mapper)
        {
            this.uploadService = uploadService;
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ResponseDto> AddNews(AddNewsDto data)
        {
            if(!await new CheckGameExists(db).GameExists(data.GameId)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Game does not exist" });
            
            var results = await uploadService.UploadPhoto(data.Photo);
            if(results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload image" });

            var mapped = mapper.Map<News>(data);
            mapped.PhotoId = results.PublicId;
            mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;

            await db.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Added news successfuly" });
            
            await uploadService.DeletePhoto(results.PublicId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add news" });
        }

        public async Task<ResponseDto> DeleteNews(Guid newsId)
        {
            var news = await db.News.FirstOrDefaultAsync(x => x.Id == newsId);
            if(news is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "News does not exist" });

            var results = await uploadService.DeletePhoto(news.PhotoId);
            if(results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete image" });

            db.News.Remove(news);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Deleted news successfuly" });
            
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete news" });
        }

        public async Task<ResponseDto> EditNews(NewsDto data)
        {
            if(!await new CheckGameExists(db).GameExists(data.GameId)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Game does not exist" });

            var news = await db.News.FirstOrDefaultAsync(x => x.Id == data.Id);
            if(news is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "News does not exist" });

            if(data.Photo is not null){
                var results = await uploadService.UploadPhoto(data.Photo);
                if(results.Error is null) {
                    await uploadService.DeletePhoto(news.PhotoId);
                    news.PhotoId = results.PublicId;
                    news.PhotoUrl = results.SecureUrl.AbsoluteUri;
                }
            }

            mapper.Map(data, news);
            db.News.Update(news);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Edited news successfuly" });
            
            await uploadService.DeletePhoto(news.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit news" });
        }

        public async Task<ResponseDto> GetGameNews(Guid gameId)
        {
            if(!await new CheckGameExists(db).GameExists(gameId)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Game does not exist" });

            IEnumerable<News> news = await db.News.Where(x => x.GameId == gameId).OrderByDescending(x => x.Id).Take(5).ToListAsync();
            
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<NewsDto>>(news));
        }
    }
}