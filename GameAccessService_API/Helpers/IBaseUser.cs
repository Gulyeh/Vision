using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Helpers
{
    public interface IBaseUser
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }
    }
}