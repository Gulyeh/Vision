using AutoMapper;
using ProductsService_API.Dtos;
using ProductsService_API.Entites;

namespace ProductsService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Products, ProductsDto>().ReverseMap();
            CreateMap<AddProductsDto, Products>();
            CreateMap<Games, GamesDto>().ReverseMap();
        }
    }
}