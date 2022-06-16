using AutoMapper;
using OrderService_API.Dtos;
using OrderService_API.Entities;
using OrderService_API.Helpers;
using OrderService_API.Messages;

namespace OrderService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<CreateOrderData, Order>();
            CreateMap<PaymentMessage, PaymentCompleted>().ForMember(x => x.IsSuccess, y => y.MapFrom(src => true));
        }
    }
}