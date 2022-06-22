using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductsService_API.DbContexts;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Helpers;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;
        private readonly ILogger<CurrencyRepository> logger;

        public CurrencyRepository(ApplicationDbContext db, IMapper mapper, ICacheService cacheService, ILogger<CurrencyRepository> logger)
        {
            this.db = db;
            this.mapper = mapper;
            this.cacheService = cacheService;
            this.logger = logger;
        }

        public async Task<ResponseDto> AddPackage(AddCurrencyDto data)
        {
            var mapped = mapper.Map<Currency>(data);
            await db.Currencies.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) {
                await cacheService.TryAddToCache<Currency>(CacheType.Currencies, mapped);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] {"Package has been added successfully"});
            }
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] {"Could not add package"});
        }

        public async Task<ResponseDto> DeletePackage(Guid packageId)
        {
            var package = await db.Currencies.FirstOrDefaultAsync(x => x.Id == packageId);
            if(package is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] {"Package does not exist"});

            db.Currencies.Remove(package);
            if(await db.SaveChangesAsync() > 0){
                var packagesCached = await cacheService.TryGetFromCache<Currency>(CacheType.Currencies);
                if(packagesCached is not null){
                    var cached = packagesCached.FirstOrDefault(x => x.Id == packageId);
                    if(cached is not null) await cacheService.DeleteFromCache<Currency>(CacheType.Currencies, cached);
                }

                logger.LogInformation("Package with ID: {x} - has been deleted successfully", packageId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] {"Package has been deleted"});
            }

            logger.LogWarning("Package with ID: {x} - could not be deleted", packageId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] {"Package could not be deleted"});
        }

        public async Task<ResponseDto> EditPackage(EditCurrencyDto data)
        {
            var package = await db.Currencies.FirstOrDefaultAsync(x => x.Id == data.Id);
            if(package is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] {"Package does not exist"});

            mapper.Map(data, package);
            if(await db.SaveChangesAsync() > 0){
                await cacheService.TryUpdateCurrency();
                logger.LogInformation("Package with ID: {x} - has been edited successfully", data.Id);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] {"Package has been edited"});
            }

            logger.LogWarning("Package with ID: {x} - could not be edited", data.Id);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] {"Package could not be edited"});
        }

        public async Task<ResponseDto> GetPackages()
        {
            IEnumerable<Currency> packages = await cacheService.TryGetFromCache<Currency>(CacheType.Currencies);
            if (packages.Count() == 0) packages = await cacheService.TryUpdateCurrency();
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<CurrencyDto>>(packages));
        }
    }
}