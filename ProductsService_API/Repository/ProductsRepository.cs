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
            if(!await gameDataService.CheckGameExists<bool>(data.GameId, Access_Token)) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            var mapped = mapper.Map<Products>(data);

            if(data.Photo is not null){
                var results = await uploadService.UploadPhoto(data.Photo);
                if(results.Error is null){
                    mapped.PhotoId = results.PublicId;
                    mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;
                }
            }

            game.GameProducts?.Add(mapped);

            if(await SaveChangesAsync()) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been added successfuly" });
            
            await uploadService.DeletePhoto(mapped.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add product" });
        }

        public async Task<ResponseDto> DeleteProduct(Guid productId, Guid gameId)
        {
            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == gameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            var product  = game.GameProducts?.FirstOrDefault(x => x.Id == productId);
            if(product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });
            
            game.GameProducts?.Remove(product);

            if(await SaveChangesAsync()) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been removed successfuly" });
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not remove product" });
        }

        public async Task<ResponseDto> EditProduct(ProductsDto data)
        {
            var oldPhotoId = string.Empty;

            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == data.GameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });
            var product  = game.GameProducts?.FirstOrDefault(x => x.Id == data.ProductId);
            if(product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });
        
            mapper.Map(data, product);

            if(data.Photo is not null){
                var results = await uploadService.UploadPhoto(data.Photo);
                if(results.Error is null){
                    oldPhotoId = product.PhotoId;
                    product.PhotoId = results.PublicId;
                    product.PhotoUrl = results.SecureUrl.AbsoluteUri;
                }
            }

            if(await SaveChangesAsync()){
                if(!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been edited successfuly" });
            }

            if(!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(product.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not edit product" });
        }

        public async Task<ResponseDto> GetProductsInGame(Guid gameId, Guid? productId = null)
        {
            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == gameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Game does not exist" });

            if(productId is not null && game.GameProducts is not null){
                var product = game.GameProducts.FirstOrDefault(x => x.Id == productId);
                return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<ProductsDto>(product));
            }
            
            else return new ResponseDto(true, StatusCodes.Status200OK, new ProductsDto());
        }

        public async Task<ResponseDto> ProductExists(Guid gameId, Guid productId)
        {
            var game = await db.Games.Include(x => x.GameProducts).FirstOrDefaultAsync(x => x.GameId == gameId);
            if(game is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new { productExists = false });
            var product  = game.GameProducts?.FirstOrDefault(x => x.Id == productId);
            if(product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new { productExists = false });
            return new ResponseDto(true, StatusCodes.Status200OK, new { productExists = true });
        }

        private async Task<bool> SaveChangesAsync(){
            if(await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}