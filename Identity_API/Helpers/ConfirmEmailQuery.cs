using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_API.Helpers
{
    public class ConfirmEmailQuery
    {
        public ConfirmEmailQuery()
        {
            this.token = string.Empty;
        }

        [Required]
        public Guid userId { get; set; }
        [Required]
        public string token { get; set; }
    }
}