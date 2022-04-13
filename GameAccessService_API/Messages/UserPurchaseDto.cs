using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Messages
{
    public class UserPurchaseDto
    {
        public UserPurchaseDto()
        {
            Email = string.Empty;
        }

        public bool isSuccess { get; set; }
        public Guid userId { get; set; }
        public Guid gameId { get; set; }
        public Guid? productId { get; set; }
        public string Email { get; set; }
    }
}