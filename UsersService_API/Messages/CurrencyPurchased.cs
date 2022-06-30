namespace UsersService_API.Messages
{
    public class CurrencyPurchased
    {
        public CurrencyPurchased(bool isSuccess, int amount)
        {
            IsSuccess = isSuccess;
            Amount = amount;
        }

        public bool IsSuccess { get; set; }
        public int Amount { get; set; }
    }
}