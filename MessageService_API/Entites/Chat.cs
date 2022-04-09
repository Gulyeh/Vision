using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Entites
{
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
        }

        public Guid Id { get; set; }
        public Guid User1 { get; set; }
        public Guid User2 { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}