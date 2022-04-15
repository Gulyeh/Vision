using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Messages
{
    public class CurrencyDoneDto
    {
        public CurrencyDoneDto()
        {
            Email = string.Empty;
        }

        public bool isSuccess { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public string Email { get; set; }
    }
}