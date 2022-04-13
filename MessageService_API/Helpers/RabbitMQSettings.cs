using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageService_API.Helpers
{
    public class RabbitMQSettings
    {
        public RabbitMQSettings()
        {
            Hostname = string.Empty;
            Password = string.Empty;
            Username = string.Empty;
        }

        public string Hostname { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}