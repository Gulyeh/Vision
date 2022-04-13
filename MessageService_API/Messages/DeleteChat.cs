using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Messages
{
    public class DeleteChat
    {
        public Guid User1 { get; set; }
        public Guid User2 { get; set; }
    }
}