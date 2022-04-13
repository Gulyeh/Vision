using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;

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