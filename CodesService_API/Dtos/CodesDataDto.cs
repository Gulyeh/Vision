using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CodesService_API.Helpers;

namespace CodesService_API.Dtos
{
    public class CodesDataDto : BasicCodes
    {
        [Required]
        public int Id { get; set; }
    }
}