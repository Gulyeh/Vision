using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService_API.Messages
{
    public class CurrencyDto
    {
        public bool isSuccess { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public string? Email { get; set; }
        public bool isCode { get; set; }
    }
}