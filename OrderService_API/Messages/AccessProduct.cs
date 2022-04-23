using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Messages
{
    public class AccessProduct
    {
        public AccessProduct()
        {
            Email = string.Empty;
        }

        public Guid userId { get; set; }
        public Guid? gameId { get; set; }
        public Guid productId { get; set; }
        public string Email { get; set; }
    }
}