using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Messages
{
    public class CurrencyDoneDto : BaseEmailData
    {
        public bool isSuccess { get; set; }
        public int Amount { get; set; }
        
    }
}