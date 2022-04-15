using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Helpers;

namespace OrderService_API.Dtos
{
    public class CurrencyDto : BaseProductData
    {
        public int Amount { get; set; }
    }
}