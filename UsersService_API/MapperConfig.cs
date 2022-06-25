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
            CreateMap<Users, GetDetailedUsersDto>();
            CreateMap<EditableUserDataDto, Users>();
            CreateMap<Users, UserDataDto>();
            CreateMap<Users, GetFriendRequestsDto>();
            CreateMap<Users, GetFriendsDto>();
            CreateMap<Users, GetPendingRequestsDto>();
            CreateMap<Users, GetUserDto>();
            CreateMap<UserDataDto, GetFriendsDto>();
            CreateMap<UserDataDto, GetUserDto>();
        }
    }
}