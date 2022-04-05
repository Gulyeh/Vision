using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using ProductsService_API.DbContexts;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Helpers;
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

        public ProductsRepository(ApplicationDbContext db, IMapper mapper, IUploadService uploadService, ICacheService cacheService)
        {
            this.db = db;
            this.mapper = mapper;
            this.uploadService = uploadService;
            this.cacheService = cacheService;
        }

        public async Task<ResponseDto> AddProduct(AddProductsDto data)
        {
            //TODO: ask gamedata server if game exists by rabbitMQ
            var results = await uploadService.UploadPhoto(data.Photo);
            if(results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload image" });
            
            var mapped = mapper.Map<Products>(data);
            mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;
            mapped.PhotoId = results.PublicId;
            
            await db.GamesProducts.AddAsync(mapped);
            if(await db.SaveChangesAsync() > 0) 
            {
                await cacheService.TryRemoveFromCache<Products>(CacheType.GameProducts, mapped.GameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been added successfuly" });
            }

            await uploadService.DeletePhoto(results.PublicId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not add product" });
        }

        public async Task<ResponseDto> DeleteProduct(Guid productId)
        {
            var product = await db.GamesProducts.FirstOrDefaultAsync(x => x.Id == productId);
            if(product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });
            
            db.GamesProducts.Remove(product);
            if(await db.SaveChangesAsync() > 0) {
                await cacheService.TryRemoveFromCache<Products>(CacheType.GameProducts, product.GameId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been deleted successfuly" });
            }
        
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete product" });
        }

        public async Task<ResponseDto> EditProduct(ProductsDto data)
        {
            string oldPhotoId = string.Empty;

            var product = await db.GamesProducts.FirstOrDefaultAsync(x => x.Id == data.Id);
            if(product is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Product does not exist" });

            if(data.Photo is not null){
                var results = await uploadService.UploadPhoto(data.Photo);
                if(results.Error is not null) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not upload image" });

                oldPhotoId = product.PhotoId;
                product.PhotoId = results.PublicId;
                product.PhotoUrl = results.SecureUrl.AbsoluteUri;
            }

            mapper.Map(data, product);
            if(await db.SaveChangesAsync() > 0){
                if(!string.IsNullOrEmpty(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Product has been updated successfuly" });
            } 

            if(!string.IsNullOrEmpty(product.PhotoId)) await uploadService.DeletePhoto(product.PhotoId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not update product" });
        }

        public async Task<ResponseDto> GetAllProducts()
        {
            var products = await db.GamesProducts.ToListAsync();
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<ProductsDto>>(products));
        }

        public async Task<ResponseDto> GetGameProducts(Guid gameId)
        {
            IEnumerable<Products> products = await cacheService.TryGetFromCache<Products>(CacheType.GameProducts, gameId);
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<ProductsDto>>(products));
        }
    }
}