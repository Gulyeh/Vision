using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using OrderService_API.Dtos;
using OrderService_API.Entities;

namespace OrderService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
        }
    }
}