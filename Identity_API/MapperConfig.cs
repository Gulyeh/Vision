using AutoMapper;
using Identity_API.DbContexts;
using Identity_API.Dtos;
using Identity_API.Entities;

namespace Identity_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<RegisterDto, ApplicationUser>().ReverseMap();
            CreateMap<BannedUsersDto, BannedUsers>().ReverseMap();
        }
    }
}