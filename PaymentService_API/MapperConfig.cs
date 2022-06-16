using AutoMapper;
using PaymentService_API.Dtos;
using PaymentService_API.Entities;
using PaymentService_API.Messages;

namespace PaymentService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<PaymentMessage, Payment>();
            CreateMap<PaymentMethods, PaymentMethodsDto>();
        }
    }
}