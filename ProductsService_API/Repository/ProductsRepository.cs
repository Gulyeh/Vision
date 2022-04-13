using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductsService_API.DbContexts;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Repository.IRepository;
using ProductsService_API.Services.IServices;

namespace ProductsService_API.Repository
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IUploadService uploadService;
        private readonly ICacheService cacheService;
        private readonly IGameDataService gameDataService;

        public ProductsRepository(ApplicationDbContext db, IMapper mapper,
            IUploadService uploadService, ICacheService cacheService, IGameDataService gameDataService)
        {
            this.db = db;
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.cacheService = cacheService;
            this.gameDataService = gameDataService;
        }

        public async Task<ResponseDto> AddProduct(AddProductsDto data, string Access_Token)
        {
            if (!await gameDataService.CheckGameExists<bool>(data.GameId, Access_Token)) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Game does not exist" });

            var results = await uploadService.UploadPhoto(data.Photo);
            if (results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload image" });

            var mapped = mapper.Map<Products>(data);
            mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;
            mapped.PhotoId = results.PublicId;

            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Could not find a game" });

            game.GameProducts?.Add(mapped);

            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<Games>(mapped.GameId, game);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been added successfuly" });
            }

            await uploadService.DeletePhoto(results.PublicId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add product" });
        }

        public async Task<ResponseDto> DeleteProduct(Guid productId)
        {
            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.Id == productId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var product = game.GameProducts?.FirstOrDefault(x => x.Id == productId);
            if (product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });

            game.GameProducts?.Remove(product);

            if (await db.SaveChangesAsync() > 0)
            {
                await cacheService.TryAddToCache<Games>(product.GameId, game);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been deleted successfuly" });
            }

            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete product" });
        }

        public async Task<ResponseDto> EditProduct(ProductsDto data)
        {
            string oldPhotoId = string.Empty;

            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.Id == data.GameId);
            if (game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var product = game.GameProducts?.FirstOrDefault(x => x.Id == data.ProductId);
            if (product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });

            if (data.Photo is not null)
            {
                var results = await uploadService.UploadPhoto(data.Photo);
                if (results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload image" });

                oldPhotoId = product.PhotoId;
                product.PhotoId = results.PublicId;
                product.PhotoUrl = results.SecureUrl.AbsoluteUri;
            }

            mapper.Map(data, product);

            if (await db.SaveChangesAsync() > 0)
            {
                if (!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);

                game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.Id == data.GameId);
                if (game is not null) await cacheService.TryAddToCache<Games>(product.GameId, game);

                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been updated successfuly" });
            }

            if (!string.IsNullOrEmpty(product.PhotoId)) await uploadService.DeletePhoto(product.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not update product" });
        }

        public async Task<ResponseDto> GetAllProducts()
        {
            var products = await db.Games.Include(x => x.GameProducts).ToListAsync();
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<GamesDto>>(products));
        }

        public async Task<ResponseDto> GetGame(Guid gameId)
        {
            Games? game = await cacheService.TryGetFromCache<Games>(gameId);
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<GamesDto>(game));
        }
    }
}