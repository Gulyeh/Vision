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
            CreateMap<Order, GetOrdersDto>().ForMember(x => x.OrderType, src => src.MapFrom(z => GetEnumString(z.OrderType)));
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<CreateOrderData, Order>();
            CreateMap<PaymentMessage, PaymentCompleted>().ForMember(x => x.IsSuccess, y => y.MapFrom(src => true));
        }

        private string GetEnumString<T>(T data)
        {
            if (data is null || !typeof(T).IsEnum) return string.Empty;
            var parsed = Enum.GetName(typeof(T), data);
            if (parsed is null) return string.Empty;
            return parsed;
        }
    }
}