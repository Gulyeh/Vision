using AutoMapper;
using GamesDataService_API.Dtos;
using GamesDataService_API.Entities;

namespace GamesDataService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Games, GamesDto>().ReverseMap();
            CreateMap<Games, AddGamesDto>().ReverseMap();
            CreateMap<AddNewsDto, News>().ReverseMap();
            CreateMap<News, NewsDto>().ReverseMap();
            CreateMap<Informations, InformationsDto>().ReverseMap();
            CreateMap<Requirements, RequirementsDto>().ReverseMap();
        }
    }
}