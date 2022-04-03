using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Helpers;

namespace GamesDataService_API.Dtos
{
    public class AddGamesDto : BaseGames
    {
        [Required]
        [NotNull]
        public IFormFile? CoverPhoto { get; set; }
        [Required]
        [NotNull]
        public IFormFile? IconPhoto { get; set; }
    }
}