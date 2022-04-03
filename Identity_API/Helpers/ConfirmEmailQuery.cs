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
            this.userId = string.Empty;
            this.token = string.Empty;
        }

        [Required]
        public string userId { get; set; }
        [Required]
        public string token { get; set; }
    }
}