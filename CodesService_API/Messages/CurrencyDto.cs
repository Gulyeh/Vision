namespace CodesService_API.Messages
{
    public class CurrencyDto
    {
        public CurrencyDto(Guid userId, int amount, bool isCode, string code)
        {
            UserId = userId;
            Amount = amount;
            this.isCode = isCode;
            Code = code;
        }

        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public bool isCode { get; set; }
        public string Code { get; set; }
    }
}