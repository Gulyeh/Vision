namespace CodesService_API.Dtos
{
    public class AppliedCodeDto
    {
        public AppliedCodeDto(string codeValue, string? signature)
        {
            CodeValue = codeValue;
            Signature = signature;
        }

        public string CodeValue { get; init; }
        public string? Signature { get; init; }
    }
}