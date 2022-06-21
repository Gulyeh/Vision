using AutoMapper;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using CodesService_API.Helpers;

namespace CodesService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<CodesDataDto, Codes>().ReverseMap();
            CreateMap<AddCodesDto, Codes>()
                .ForMember(x => x.CodeType, src => src.Ignore())
                .ForMember(x => x.Signature, src => src.Ignore());
        }
    }
}