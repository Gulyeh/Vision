using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MessageService_API.Dtos;
using MessageService_API.Entites;

namespace MessageService_API
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Message, MessageDto>();
            CreateMap<AddMessageDto, Message>();
            CreateMap<MessageAttachment, MessageAttachmentDto>();
            CreateMap<CreateChatDto, Chat>();
        }
    }
}