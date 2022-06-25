using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class DetailedUserModel
    {
        public DetailedUserModel()
        {
            PhotoUrl = string.Empty;
            Description = string.Empty;
            Username = string.Empty;
        }

        public Guid UserId { get; set; }
        public string PhotoUrl { get; set; }
        public string Description { get; set; }
        public string Username { get; set; }
        public int CurrencyValue { get; set; }
        public bool IsDeletedAccount { get; set; }
        public bool IsBanned { get; set; }
    }
}
