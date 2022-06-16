namespace UsersService_API.Messages
{
    public class CurrencyDto
    {
        public CurrencyDto()
        {
            Code = string.Empty;
        }

        public bool IsSuccess { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public string? Email { get; set; }
        public bool IsCode { get; set; }
        public string Code { get; set; }
    }
}