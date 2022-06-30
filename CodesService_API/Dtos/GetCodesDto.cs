namespace CodesService_API.Dtos
{
    public class GetCodesDto
    {
        public GetCodesDto()
        {
            Code = string.Empty;
            CodeValue = string.Empty;
            Signature = string.Empty;
            CodeType = string.Empty;
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public string CodeValue { get; set; }
        public string Signature { get; set; }
        public Guid? GameId { get; set; }
        public string CodeType { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsLimited { get; set; }
        public int Uses { get; set; }
    }
}