using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity_API.Helpers;

namespace Identity_API.Messages.Interfaces
{
    public interface IEmailDataDto
    {
        public Guid userId { get; set; }
        public string Content { get; set; }
        public string ReceiverEmail { get; set; }
        public EmailTypes EmailType { get; set; }
    }
}