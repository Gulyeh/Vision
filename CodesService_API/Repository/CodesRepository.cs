using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodesService_API.DbContexts;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Helpers;
using CodesService_API.Repository.IRepository;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using CodesService_API.Extensions;

namespace CodesService_API.Repository
{
    public class CodesRepository : ICodesRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IHashids hashids;

        public CodesRepository(ApplicationDbContext db, IMapper mapper, IHashids hashids)
        {
            this.db = db;
            this.mapper = mapper;
            this.hashids = hashids;
        }

        public async Task<ResponseDto> CheckCode(string code)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Code == code);
            if(checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
            if(checkCode.ExpireDate <= DateTime.Now) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" });

            var mapped = mapper.Map<CodesDto>(checkCode);
            return new ResponseDto(true, StatusCodes.Status200OK, mapped);
        }

        public async Task<ResponseDto> EditCode(CodesDataDto codeData)
        {
            var decodedId = new DecodeHash(hashids).Decode(codeData.Id);
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Id == decodedId);
            if(checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
            
            mapper.Map(codeData, checkCode);
            db.Entry(checkCode).State = EntityState.Modified;
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been modified successfuly" });
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not modify code" });
        }

        public async Task<ResponseDto> GetAllCodes()
        {
            var codes = await db.Codes.ToListAsync();
            var mapped = mapper.Map<List<CodesDataDto>>(codes);
            mapped = mapped.ListIdsHasher(hashids);
            return new ResponseDto(true, StatusCodes.Status200OK, mapped);
        }

        public async Task<ResponseDto> RemoveCode(string code)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Code == code);
            if(checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            db.Codes.Remove(checkCode);
            if(await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been deleted successfuly" });
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete code" });
        }
    }
}