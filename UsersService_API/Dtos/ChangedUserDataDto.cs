using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService_API.Dtos
{
    public class ChangedUserDataDto
    {
        public ChangedUserDataDto(Guid userId, string description, string nickname)
        {
            this.userId = userId;
            Description = description;
            Nickname = nickname;
        }

        public Guid userId { get; private set; }
        public string Description { get; private set; }
        public string Nickname { get; private set; }
    }
}