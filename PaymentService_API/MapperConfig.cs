using AutoMapper;
using PaymentService_API.Dtos;
using PaymentService_API.Entities;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;

namespace PaymentService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<PaymentMessage, Payment>();
            CreateMap<PaymentMethods, PaymentMethodsDto>();
            CreateMap<AddPaymentMethodDto, PaymentMethods>()
                .ForMember(x => x.Provider, src => src.MapFrom(z => Enum.Parse(typeof(PaymentProvider), z.Provider)))
                .ForMember(x => x.PhotoId, src => src.Ignore())
                .ForMember(x => x.PhotoUrl, src => src.Ignore())
                .ForMember(x => x.Id, src => src.Ignore());
        }
    }
}