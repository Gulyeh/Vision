using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Helpers;

namespace GamesDataService_API.Entities
{
    public class Games : BaseGames
    {
        public Games()
        {
            CoverId = string.Empty;
            IconId = string.Empty;
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public string IconId { get; set; }
        [Required]
        public string CoverId { get; set; }
        public ICollection<News>? News { get; set; }
    }
}