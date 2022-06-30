using ProductsService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace ProductsService_API.Entites
{
    public class Games : BaseProducts
    {
        public Games()
        {
            PhotoId = string.Empty;
            PhotoUrl = string.Empty;
            Products = new List<Products>();
        }

        [Required]
        public Guid GameId { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        [Required]
        public string PhotoId { get; set; }
        public ICollection<Products> Products { get; set; }
    }
}