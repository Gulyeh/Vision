using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersService_API.Helpers;

namespace UsersService_API.Dtos
{
    public class ChangedUserStatusDto
    {
        public ChangedUserStatusDto(Guid userId, Status status)
        {
            this.userId = userId;
            this.status = status;
        }

        public Guid userId { get; private set; }
        public Status status { get; private set; }
    }
}