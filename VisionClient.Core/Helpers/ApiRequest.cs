using VisionClient.Core.Enums;

namespace VisionClient.Core.Helpers
{
    public class ApiRequest
    {
        public ApiRequest()
        {
            ApiUrl = string.Empty;
            Access_Token = string.Empty;
        }

        public APIType ApiType { get; set; }
        public object? Data { get; set; }
        public string ApiUrl { get; set; }
        public string Access_Token { get; set; }
    }
}
