using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService_API.Messages
{
    public class UserGameDto
    {
        public Guid userId { get; set; }
        public Guid gameId { get; set; }
    }
}