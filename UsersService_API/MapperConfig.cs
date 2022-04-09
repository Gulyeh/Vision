using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using UsersService_API.Dtos;
using UsersService_API.Entites;

namespace UsersService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<FriendRequestDto, FriendRequests>();
            CreateMap<EditableUserDataDto, Users>();
            CreateMap<Users, UserDataDto>();
        }
    }
}