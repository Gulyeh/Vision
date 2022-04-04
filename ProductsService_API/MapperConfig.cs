using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }
    }
}