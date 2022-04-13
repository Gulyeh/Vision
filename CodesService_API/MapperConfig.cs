using AutoMapper;
using CodesService_API.Dtos;
using CodesService_API.Entites;

namespace CodesService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<CodesDataDto, Codes>().ReverseMap();
            CreateMap<AddCodesDto, Codes>();
        }
    }
}