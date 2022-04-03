using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Helpers;

namespace GamesDataService_API.Entities
{
    public class News : BaseNews
    {
        public News()
        {
            Game = new Games();
            PhotoId = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string PhotoId { get; set; }
        [ForeignKey("GameId")]
        public Games Game { get; set;}
    }
}