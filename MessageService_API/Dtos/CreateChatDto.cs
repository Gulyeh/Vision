using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Dtos
{
    public class CreateChatDto
    {
        public Guid User1 { get; set; }
        public Guid User2 { get; set; }
    }
}