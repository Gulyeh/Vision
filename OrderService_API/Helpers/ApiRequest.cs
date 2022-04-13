namespace OrderService_API.Helpers
{
    public class ApiRequest
    {
        public ApiRequest()
        {
            Data = new();
            ApiUrl = string.Empty;
            Access_Token = string.Empty;
        }

        public APIType apiType { get; set; }
        public object Data { get; set; }
        public string ApiUrl { get; set; }
        public string Access_Token { get; set; }
    }
}