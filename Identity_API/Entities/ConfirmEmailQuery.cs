using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Identity_API.Entities
{
    public class ConfirmEmailQuery
    {
        public ConfirmEmailQuery(string userId, string token)
        {
            this.userId = userId;
            this.token = token;
        }

        [Required]
        public string userId { get; private set; }
        [Required]
        public string token { get; private set; }
    }
}