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
            CreateMap<EditCodeDto, Codes>().ReverseMap();
            CreateMap<AddCodesDto, Codes>()
                .ForMember(x => x.CodeType, src => src.Ignore())
                .ForMember(x => x.Signature, src => src.Ignore());
            CreateMap<Codes, GetCodesDto>()
                .ForMember(x => x.CodeType, src => src.MapFrom(z => GetEnumString(z.CodeType)))
                .ForMember(x => x.Signature, src => src.MapFrom(z => GetEnumString(z.Signature)));
            CreateMap<CodesUsed, GetUserUsedCodesDto>()
                .ForMember(x => x.CodeType, src => src.MapFrom(y => GetEnumString(y.Code.CodeType)))
                .ForMember(x => x.Code, src => src.MapFrom(z => z.Code.Code));
        }

        private string GetEnumString<T>(T data)
        {
            if(data is null || !typeof(T).IsEnum) return string.Empty;
            var parsed = Enum.GetName(typeof(T), data);
            if(parsed is null) return string.Empty;
            return parsed;
        }
    }
}