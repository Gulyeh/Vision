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
            CreateMap<EditPaymentMethodDto, PaymentMethods>();
            CreateMap<PaymentMethods, PaymentMethodsDto>().ForMember(x => x.Title, src => src.MapFrom(z => z.Provider.ToString()));
            CreateMap<AddPaymentMethodDto, PaymentMethods>()
                .ForMember(x => x.Provider, src => src.Ignore())
                .ForMember(x => x.PhotoId, src => src.Ignore())
                .ForMember(x => x.PhotoUrl, src => src.Ignore())
                .ForMember(x => x.Id, src => src.Ignore());
        }
    }
}