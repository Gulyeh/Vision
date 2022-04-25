using AutoMapper;
using OrderService_API.Dtos;
using OrderService_API.Entities;
using OrderService_API.Helpers;

namespace OrderService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<CreateOrderData, Order>();
        }
    }
}