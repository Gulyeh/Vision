using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ProductsService_API.Helpers;

namespace ProductsService_API.Entites
{
    public class Products : BaseProducts
    {
        public Products()
        {
            PhotoUrl = string.Empty;
            PhotoId = string.Empty;
            Game = new Games();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PhotoId { get; set; }
        
        public Guid GameId { get; set; }
        [ForeignKey("GameId")]
        public Games Game { get; set; }
    }
}