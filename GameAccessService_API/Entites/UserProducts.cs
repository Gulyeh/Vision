using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameAccessService_API.Helpers;

namespace GameAccessService_API.Entites
{
    public class UserProducts : BaseUser
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
    }
}