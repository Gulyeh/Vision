using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersService_API.Helpers
{
    public class CloudinarySettings
    {
        public CloudinarySettings()
        {
            CloudName = string.Empty;
            ApiKey = string.Empty;
            ApiSecret = string.Empty;
        }

        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }
}