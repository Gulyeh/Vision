using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GamesDataService_API.Helpers
{
    public class BaseGames
    {
        public BaseGames()
        {
            Name = string.Empty;
            IconUrl = string.Empty;
            CoverUrl = string.Empty;
            ClientVersion = string.Empty;
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string IconUrl { get; set; }
        [Required]
        public string CoverUrl { get; set; }
        [Required]
        public string ClientVersion { get; set; }
    }
}