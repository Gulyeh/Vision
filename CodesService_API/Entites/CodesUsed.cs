using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Entites
{
    public class CodesUsed
    {
        public CodesUsed()
        {
            Code = new Codes();
        }

        [Key]
        public int Id { get; set; }
        [Required]
        public Guid userId { get; set; }
        
        public int CodeId { get; set; }
        [ForeignKey("CodeId")]
        public Codes Code { get ;set; }
    }
}