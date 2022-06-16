namespace GameAccessService_API.Messages
{
    public class CodeFailedDto
    {
        public CodeFailedDto(Guid userId, string code)
        {
            UserId = userId;
            Code = code;
        }

        public Guid UserId { get; set; }
        public string Code { get; set; }
    }
}