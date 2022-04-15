using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Entites
{
    public class Currency : BaseProducts
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }       
    }
}