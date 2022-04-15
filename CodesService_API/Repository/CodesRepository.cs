using AutoMapper;
using CodesService_API.DbContexts;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Helpers;
using CodesService_API.RabbitMQSender;
using CodesService_API.Repository.IRepository;
using CodesService_API.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace CodesService_API.Repository
{
    public class CodesRepository : ICodesRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IGameAccessService gameAccessService;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly IRabbitMQSender rabbitMQSender;

        public CodesRepository(ApplicationDbContext db, IGameAccessService gameAccessService, IMapper mapper, ICacheService cacheService, IRabbitMQSender rabbitMQSender)
        {
            this.db = db;
            this.gameAccessService = gameAccessService;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<ResponseDto> ApplyCode(string code, Guid userId, CodeTypes codeType, string Access_Token){
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);
            var checkCode = Codes.FirstOrDefault(c => c.Code == code);

            var validate = await ValidateCode(checkCode, codeType);
            if(validate.isSuccess == false) return validate;

            var dbcode = await db.Codes.Include(x => x.CodesUsed).FirstOrDefaultAsync(x => x.Code == code);
            var codeUsed = dbcode?.CodesUsed.FirstOrDefault(x => x.userId == userId);
            if(codeUsed is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code has been already used" });      

            if (dbcode?.isLimited == true) dbcode.Uses--;

            if(codeType == CodeTypes.Game) {
                if(await gameAccessService.CheckAccess(checkCode?.CodeValue, Access_Token)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You already have this game" });      
            }
            else if(codeType == CodeTypes.Product){
                if(await gameAccessService.CheckAccess(checkCode?.gameId.ToString(), Access_Token, checkCode?.CodeValue)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You already have this product" });
            } 

            dbcode?.CodesUsed.Add(new CodesUsed(){
                userId = userId
            });

            if (await db.SaveChangesAsync() > 0){

                if(codeType == CodeTypes.Game){
                    rabbitMQSender.SendMessage(new { userId = userId, gameId = checkCode?.CodeValue}, "ProductCuponUsedQueue");
                }
                else if(codeType == CodeTypes.Product){
                    rabbitMQSender.SendMessage(new { userId = userId, gameId = checkCode?.gameId, productId = checkCode?.CodeValue }, "ProductCuponUsedQueue");
                }
                else if(codeType == CodeTypes.Currency) rabbitMQSender.SendMessage(new { userId = userId, productId = checkCode?.CodeValue, isCode = true }, "ChangeFundsQueue");

                return new ResponseDto(true, StatusCodes.Status200OK, new { Message = "You have redeemed code successfuly", codeValue = checkCode?.CodeValue });
            }
            
            return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
        }

        public async Task<ResponseDto> AddCode(AddCodesDto code)
        {
            var mapped = mapper.Map<Codes>(code);
            await db.Codes.AddAsync(mapped);
            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<Codes>(CacheType.Codes, mapped);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been added successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add code" });
        }

        public async Task<ResponseDto> CheckCode(string code, CodeTypes codeType)
        {
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);
            var checkCode = Codes.FirstOrDefault(c => c.Code == code);

            var validate = await ValidateCode(checkCode, codeType);
            if(validate.isSuccess == false) return validate;

            return new ResponseDto(true, StatusCodes.Status200OK, new { codeValue = checkCode?.CodeValue });
        }

        public async Task<ResponseDto> EditCode(CodesDataDto codeData)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Id == codeData.Id);
            if (checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            mapper.Map(codeData, checkCode);
            if (await db.SaveChangesAsync() > 0) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been modified successfuly" });
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
            if (checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            db.Codes.Remove(checkCode);
            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryRemoveFromCache<Codes>(CacheType.Codes, checkCode);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been deleted successfuly" });
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete code" });
        }

        private Task<ResponseDto> ValidateCode(Codes? checkCode, CodeTypes codeType){
            if (checkCode is null) return Task.FromResult(new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" }));
            if (checkCode.ExpireDate <= DateTime.Now) return Task.FromResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" }));
            if (checkCode.isLimited == true && checkCode.Uses == 0) return Task.FromResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" }));
            if (checkCode?.CodeType != codeType) return Task.FromResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code does not exist" }));
            return Task.FromResult(new ResponseDto(true, StatusCodes.Status200OK, new{}));
        }
    }
}