using AutoMapper;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;
using ProductsService_API.Messages;

namespace ProductsService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<ProductsDto, Products>().ReverseMap();
            CreateMap<AddProductsDto, Products>();
            CreateMap<Games, GamesDto>().ReverseMap();
            CreateMap<Currency, CurrencyDto>().ReverseMap();
            CreateMap<NewProductDto, Games>();
            CreateMap<NewProductDto, Products>();
        }
    }
}