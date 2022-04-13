using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Entites
{
    public class Games : BaseProducts
    {   
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid GameId { get; set; }
        public ICollection<Products>? GameProducts { get; set; }
    }
}