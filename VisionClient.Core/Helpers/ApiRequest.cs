using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Helpers
{
    public class ApiRequest
    {
        public ApiRequest()
        {
            Data = new();
            ApiUrl = string.Empty;
            Access_Token = StaticData.Access_Token;
        }

        public APIType ApiType { get; set; }
        public object Data { get; set; }
        public string ApiUrl { get; set; }
        public string Access_Token { get; private set; }
    }
}
