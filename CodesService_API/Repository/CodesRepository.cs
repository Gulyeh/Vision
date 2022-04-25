using AutoMapper;
using CodesService_API.Builders;
using CodesService_API.DbContexts;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Helpers;
using CodesService_API.Processor;
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
        private readonly ILogger<CodesRepository> logger;

        public CodesRepository(ApplicationDbContext db, IGameAccessService gameAccessService, IMapper mapper, ICacheService cacheService, IRabbitMQSender rabbitMQSender, ILogger<CodesRepository> logger)
        {
            this.db = db;
            this.gameAccessService = gameAccessService;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
        }

        public async Task<ResponseDto> ApplyCode(string code, Guid userId, CodeTypes codeType, string Access_Token){
            var dbcode = await db.Codes.Include(x => x.CodesUsed).FirstOrDefaultAsync(x => x.Code == code);
            if (dbcode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
            
            var validate = await ValidateCode(dbcode, codeType);
            if(validate.isSuccess == false) return validate;

            var codeUsed = dbcode.CodesUsed.FirstOrDefault(x => x.userId == userId);
            if(codeUsed is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code has been already used" });      

            var codeProccessor = new CodeTypeProcessor(gameAccessService, rabbitMQSender);
            var codeAccess = codeProccessor.CreateAccessCode(codeType);
            if(codeAccess is not null && await codeAccess.CheckAccess(dbcode.gameId.ToString(), Access_Token, dbcode.CodeValue)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You already own this product" });
 
            if (dbcode.isLimited == true) dbcode.Uses--;

            dbcode.CodesUsed.Add(new CodesUsed(){
                userId = userId
            });

            if (await db.SaveChangesAsync() > 0){
                var sender = codeProccessor.CreateSenderCode(codeType);
                if(sender is not null){
                    sender.SendRabbitMQMessage(userId, dbcode.gameId, dbcode.CodeValue);
                    logger.LogInformation("User: {userId} has applied code: {code} with a code type: {codeType}", userId, code, codeType);
                    return sender.GetResponse(dbcode);
                }
            }

            logger.LogError("User: {userId} had error while applying code: {code}", userId, code);
            return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
        }

        public async Task<ResponseDto> AddCode(AddCodesDto code)
        {
            var mapped = mapper.Map<Codes>(code);
            await db.Codes.AddAsync(mapped);
            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<Codes>(CacheType.Codes, mapped);
                logger.LogInformation("Code: {code} has been added", code.Code);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been added successfuly" });
            }
            logger.LogError("Code: {code} could not be added", code.Code);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add code" });
        }

        public async Task<ResponseDto> CheckCode(string code, CodeTypes codeType)
        {
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);
            var checkCode = Codes.FirstOrDefault(c => c.Code == code);
            if (checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            var validate = await ValidateCode(checkCode, codeType);
            if(validate.isSuccess == false) return validate;

            var codeResponseBuilder = new ResponseCodeBuilder();
            codeResponseBuilder.SetCodeType(codeType);
            codeResponseBuilder.SetGame(checkCode.gameId);
            codeResponseBuilder.SetProduct(checkCode.CodeValue);
            codeResponseBuilder.SetTitle(checkCode.Title);
            var codeResponse = codeResponseBuilder.Build();

            return new ResponseDto(true, StatusCodes.Status200OK, codeResponse);
        }

        public async Task<ResponseDto> EditCode(CodesDataDto codeData)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Id == codeData.Id);
            if (checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            mapper.Map(codeData, checkCode);
            if (await db.SaveChangesAsync() > 0) {
                logger.LogInformation("Code: {code} has been modified", codeData.Code);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been modified successfuly" });
            }

            logger.LogError("Code: {code} could not be modified", codeData.Code);
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
                logger.LogInformation("Code: {code} has been removed", checkCode.Code);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Code has been deleted successfuly" });
            }

            logger.LogInformation("Code: {code} could not be removed", checkCode.Code);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete code" });
        }

        private Task<ResponseDto> ValidateCode(Codes checkCode, CodeTypes codeType){
            if (checkCode.ExpireDate <= DateTime.Now) return Task.FromResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" }));
            if (checkCode.isLimited == true && checkCode.Uses == 0) return Task.FromResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" }));
            if (checkCode.CodeType != codeType) return Task.FromResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code does not exist" }));
            return Task.FromResult(new ResponseDto(true, StatusCodes.Status200OK, new{}));
        }
    }
}