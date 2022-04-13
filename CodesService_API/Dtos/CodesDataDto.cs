using CodesService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CodesService_API.Dtos
{
    public class CodesDataDto : BasicCodes
    {
        [Required]
        public int Id { get; set; }
    }
}