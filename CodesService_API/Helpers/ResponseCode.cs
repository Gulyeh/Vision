using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodesService_API.Helpers
{
    public class ResponseCode
    {
        public ResponseCode()
        {
            Title = string.Empty;
        }

        public string Title { get; set; }
        public string? ProductId { get; set; }
        public Guid? GameId { get; set; }
        public CodeTypes CodeType { get; set; }
    }   
}