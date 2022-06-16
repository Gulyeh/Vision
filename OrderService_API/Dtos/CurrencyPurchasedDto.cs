namespace OrderService_API.Dtos
{
    public class CurrencyPurchasedDto
    {
        public CurrencyPurchasedDto(bool isSuccess, int amount)
        {
            IsSuccess = isSuccess;
            Amount = amount;
        }

        public bool IsSuccess { get; private set; }
        public int Amount { get; private set; }
    }
}