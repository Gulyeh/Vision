namespace VisionClient.Core.Models
{
    public class UsedCodeModel
    {
        public UsedCodeModel()
        {
            Code = string.Empty;
            CodeType = string.Empty;
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string CodeType { get; set; }
    }
}
