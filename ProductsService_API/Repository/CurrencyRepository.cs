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

        public async Task<ResponseDto> GetPackages()
        {
            IEnumerable<Currency> packages = await cacheService.TryGetFromCache<Currency>(CacheType.Currencies);
            if (packages.Count() == 0)
            {
                var packagesDB = await db.Currencies.ToListAsync();
                foreach (var package in packagesDB) await cacheService.TryAddToCache<Currency>(CacheType.Currencies, package);
                packages = packagesDB;
            }
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<CurrencyDto>>(packages));
        }
    }
}