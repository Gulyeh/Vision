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
        private readonly ICodeTypeProcessor codeTypeProcessor;

        public CodesRepository(ApplicationDbContext db, IGameAccessService gameAccessService, IMapper mapper, ICacheService cacheService, IRabbitMQSender rabbitMQSender,
            ILogger<CodesRepository> logger, ICodeTypeProcessor codeTypeProcessor)
        {
            this.db = db;
            this.gameAccessService = gameAccessService;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.codeTypeProcessor = codeTypeProcessor;
        }

        public async Task<ResponseDto> ApplyCode(string code, Guid userId, CodeTypes codeType, string Access_Token)
        {
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);
            var validate = await ValidateCode(Codes, codeType, userId, code);
            if (validate.isSuccess == false) return validate;

            var dbcode = await db.Codes.FirstAsync(x => x.Code.Equals(code));
            var codeAccess = codeTypeProcessor.CreateAccessCode(codeType);
            if (codeAccess is not null && await codeAccess.CheckAccess(dbcode.GameId, Access_Token, dbcode.CodeValue)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "You cannot redeem this coupon" });

            if (dbcode.IsLimited == true) dbcode.Uses--;

            dbcode.CodesUsed.Add(new CodesUsed()
            {
                userId = userId
            });

            if (await db.SaveChangesAsync() > 0)
            {
                logger.LogInformation("User: {userId} has applied code: {code}", userId, code);

                await cacheService.TryAddToCache(CacheType.CodesUsed, dbcode.CodesUsed.First());
                var sender = codeTypeProcessor.CreateSenderCode(codeType);
                if (sender is not null)
                {
                    sender.SendRabbitMQMessage(userId, dbcode.GameId, dbcode.CodeValue, dbcode.Code);
                    return new ResponseDto(true, StatusCodes.Status200OK, new { });
                }

                string? enumString = string.Empty;
                if (dbcode.Signature is not null) enumString = Enum.GetName(typeof(Signatures), dbcode.Signature);
                return new ResponseDto(true, StatusCodes.Status200OK, new { CodeValue = dbcode.CodeValue, Signature = enumString });
            }

            logger.LogError("User: {userId} had error while applying code: {code}", userId, code);
            return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });
        }

        public async Task<ResponseDto> AddCode(AddCodesDto code)
        {
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);
            if(Codes.Any(x => x.Code.Equals(code.Code))) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code already exists" });
            
            var isTypeParsed = Enum.TryParse(code.CodeType, true, out CodeTypes typeParsed);
            var isSignatureParsed = Enum.TryParse(code.Signature, true, out Signatures signatureParsed);
            if(!isTypeParsed || (typeParsed == CodeTypes.Discount && !isSignatureParsed)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not parse data" });
            
            var mapped = mapper.Map<Codes>(code);
            mapped.CodeType = typeParsed;
            if(typeParsed == CodeTypes.Discount) mapped.Signature = signatureParsed;

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

        public async Task<ResponseDto> CheckCode(string code, CodeTypes codeType, Guid userId)
        {
            IEnumerable<Codes> Codes = await cacheService.TryGetFromCache<Codes>(CacheType.Codes);
            var validate = await ValidateCode(Codes, codeType, userId, code);
            if (validate.isSuccess == false) return validate;

            var checkCode = Codes.First(c => c.Code == code);

            var codeResponseBuilder = new ResponseCodeBuilder();
            codeResponseBuilder.SetCoupon(checkCode.Code);
            codeResponseBuilder.SetCodeType(codeType);
            codeResponseBuilder.SetGame(checkCode.GameId);
            codeResponseBuilder.SetCodeValue(checkCode.CodeValue);
            codeResponseBuilder.SetSignature(checkCode.Signature);
            var codeResponse = codeResponseBuilder.Build();

            return new ResponseDto(true, StatusCodes.Status200OK, codeResponse);
        }

        public async Task<ResponseDto> EditCode(CodesDataDto codeData)
        {
            var checkCode = await db.Codes.FirstOrDefaultAsync(c => c.Id == codeData.Id);
            if (checkCode is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Code does not exist" });

            mapper.Map(codeData, checkCode);
            if (await db.SaveChangesAsync() > 0)
            {
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

        private async Task<ResponseDto> ValidateCode(IEnumerable<Codes> Codes, CodeTypes codeType, Guid userId, string code)
        {
            IEnumerable<CodesUsed> CodesUsed = await cacheService.TryGetFromCache<CodesUsed>(CacheType.CodesUsed);
            var checkCode = Codes.FirstOrDefault(x => x.Code.Equals(code));

            if (checkCode is null || checkCode.CodeType != codeType) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code does not exist" });
            if (checkCode.ExpireDate <= DateTime.Now || (checkCode.IsLimited == true && checkCode.Uses == 0)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code expired" });
            if (CodesUsed.FirstOrDefault(x => x.userId == userId && x.CodeId == checkCode.Id) is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Code has been already used" });
            return new ResponseDto(true, StatusCodes.Status200OK, new { });
        }

        public async Task RemoveUsedCode(string code, Guid userId)
        {
            var codeFound = await db.Codes.Include(x => x.CodesUsed).FirstOrDefaultAsync(x => x.Code.Equals(code));
            if (codeFound is not null)
            {
                var usedCode = codeFound.CodesUsed.FirstOrDefault(x => x.userId == userId);
                if (usedCode is not null)
                {
                    codeFound.CodesUsed.Remove(usedCode);
                    if (codeFound.IsLimited) codeFound.Uses++;
                    if (await db.SaveChangesAsync() > 0)
                    {
                        await cacheService.TryRemoveFromCache(CacheType.CodesUsed, usedCode);
                        logger.LogInformation("Code: {code} has been removed from Used Codes for user: {userId}", code, userId);
                    }
                }
            }
        }
    }
}