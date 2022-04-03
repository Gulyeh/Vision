using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CodesService_API.Dtos;
using CodesService_API.Entites;
using HashidsNet;

namespace CodesService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Codes, CodesDto>().ReverseMap();
            CreateMap<CodesDataDto, Codes>().ReverseMap();
        }
    }
}