using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Identity_API.Dtos;
using Identity_API.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<RegisterDto, IdentityUser>().ReverseMap();
            CreateMap<BannedUsersDto, BannedUsers>().ReverseMap();
        }
    }
}