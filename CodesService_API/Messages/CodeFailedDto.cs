namespace CodesService_API.Messages
{
    public class CodeFailedDto
    {
        public CodeFailedDto()
        {
            Code = string.Empty;
        }

        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}