using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodesService_API.Entites
{
    public class CodesUsed
    {
        public CodesUsed()
        {
            Code = new Codes();
        }

        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public Guid CodeId { get; set; }
        [ForeignKey("CodeId")]
        public Codes Code { get; set; }
    }
}