using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Dtos
{
    public class GamesDto : BaseProducts
    {
        [Required]
        public Guid GameId { get; set; }
        public ICollection<ProductsDto>? GameProducts { get; set; }
    }
}