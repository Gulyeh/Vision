using CodesService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CodesService_API.Entites
{
    public class Codes : BasicCodes
    {
        [Key]
        public int Id { get; set; }
    }
}