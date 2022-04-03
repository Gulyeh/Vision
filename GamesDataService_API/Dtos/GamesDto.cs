using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Helpers;

namespace GamesDataService_API.Dtos
{
    public class GamesDto : BaseGames
    {
        [Required]
        public Guid Id { get; set; }
        public IFormFile? CoverPhoto { get; set; }
        public IFormFile? IconPhoto { get; set; }
    }
}