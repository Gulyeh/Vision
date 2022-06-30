namespace OrderService_API.Messages
{
    public class UpdateFunds
    {
        public UpdateFunds()
        {
            Email = string.Empty;
        }

        public Guid userId { get; set; }
        public int Amount { get; set; }
        public string Email { get; set; }
        public bool isCode { get; set; }
    }
}