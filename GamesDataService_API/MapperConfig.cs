using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GamesDataService_API.Dtos;
using GamesDataService_API.Entities;
using HashidsNet;

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
        }
    }
}