namespace OrderService_API.Messages
{
    public class CurrencyDoneDto
    {
        public bool IsSuccess { get; set; }
        public int Amount { get; set; }
        public Guid UserId { get; set; }

    }
}