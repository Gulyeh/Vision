namespace PaymentService_API.Dtos
{
    public class PaymentMethodsDto
    {
        public PaymentMethodsDto()
        {
            PhotoUrl = string.Empty;
        }

        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}