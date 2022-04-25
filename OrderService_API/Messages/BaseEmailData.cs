using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Messages
{
    public class BaseEmailData
    {
        public BaseEmailData()
        {
            Email = string.Empty;
        }

        public string Email { get; set; }
        public Guid UserId { get; set; }
    }
}