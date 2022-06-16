namespace CodesService_API.Helpers
{
    public class ApiRequest
    {
        public ApiRequest()
        {
            Data = new();
            ApiUrl = string.Empty;
            access_token = string.Empty;
        }

        public APIType apiType { get; set; }
        public object Data { get; set; }
        public string ApiUrl { get; set; }
        private string access_token;
        public string Access_Token
        {
            get => access_token;
            set => access_token = value.Replace("Bearer ", "");
        }
    }
}