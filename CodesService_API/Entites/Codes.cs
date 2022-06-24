using CodesService_API.Helpers;
using System.ComponentModel.DataAnnotations;

namespace CodesService_API.Entites
{
    public class Codes : BasicCodes
    {
        public Codes()
        {
            CodesUsed = new List<CodesUsed>();
        }

        [Key]
        public Guid Id { get; set; }
        public ICollection<CodesUsed> CodesUsed { get; set; }
    }
}