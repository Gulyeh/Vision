using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PaymentService_API.Entities;
using PaymentService_API.Messages;

namespace PaymentService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<PaymentMessage, Payment>();
        }
    }
}