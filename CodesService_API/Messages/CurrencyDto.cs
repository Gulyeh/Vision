using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Messages
{
    public class CurrencyDto
    {
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public bool isCode { get; set; }
    }
}