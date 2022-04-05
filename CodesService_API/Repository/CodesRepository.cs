using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodesService_API.DbContexts;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Repository.IRepository;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using CodesService_API.Extensions;
using Microsoft.Extensions.Caching.Memory;
using CodesService_API.Services.IServices;
using CodesService_API.Helpers;

namespace CodesService_API.Repository
{
    public class CodesRepository : ICodesRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;

        public CodesRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService)
        {
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public async Task<ResponseDto> AddCode(AddCodesDto code)
        {
            var mapped = mapper.Map<Codes>(code);
            await db.Codes.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) 
            {
                await cacheService.TryAddToCache<Codes>(CacheType.Codes, mapped);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been added successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add code" });
        }

        public async Task<ResponseDto> CheckCode(string code)
        {
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);            
            var checkCode = Codes.FirstOrDefault(c => c.Code == code);
            if(checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
            if(checkCode.ExpireDate <= DateTime.Now) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" });

            return new ResponseDto(true, StatusCodes.Status200OK, new { codeValue = checkCode.CodeValue });
        }

        public async Task<ResponseDto> EditCode(CodesDataDto codeData)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Id == codeData.Id);
            if(checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
            
            mapper.Map(codeData, checkCode);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been modified successfuly" });
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not modify code" });
        }

        public async Task<ResponseDto> GetAllCodes()
        {
            var codes = await db.Codes.ToListAsync();
            var mapped = mapper.Map<List<CodesDataDto>>(codes);
            return new ResponseDto(true, StatusCodes.Status200OK, mapped);
        }

        public async Task<ResponseDto> RemoveCode(int codeId)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Id == codeId);
            if(checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            db.Codes.Remove(checkCode);
            if(await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryRemoveFromCache<Codes>(CacheType.Codes, checkCode);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been deleted successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete code" });
        }
    }
}