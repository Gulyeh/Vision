namespace VisionClient.Core.Helpers.Aggregators
{
    public class CodeResponse
    {
        public CodeResponse(string response)
        {
            Response = response;
        }

        public string Response { get; set; }
    }
}
