using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Enums;

namespace VisionClient.Core.Models
{
    public class UserModel
    {
        public UserModel()
        {
            UserName = string.Empty;
            PhotoUrl = string.Empty;
            EmailAddress = string.Empty;
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string? Description { get; set; }
        public string EmailAddress { get; set; }
        public string PhotoUrl { get; set; }
        public Status Status { get; set; }
        public bool IsBlocked { get; set; }
    }
}
