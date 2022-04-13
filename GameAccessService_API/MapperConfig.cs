using AutoMapper;
using GameAccessService_API.Dtos;
using GameAccessService_API.Entites;

namespace GameAccessService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<UserAccess, UserAccessDto>().ReverseMap();
        }
    }
}