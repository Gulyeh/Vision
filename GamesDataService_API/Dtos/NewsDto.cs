using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Helpers;

namespace GamesDataService_API.Dtos
{
    public class NewsDto : BaseNews
    {
        [Required]
        public Guid Id { get; set; }
        public IFormFile? Photo { get; set; }
    }
}