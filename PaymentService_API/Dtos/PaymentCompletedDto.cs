namespace PaymentService_API.Dtos
{
    public class PaymentCompletedDto
    {
        public PaymentCompletedDto()
        {
            SessionId = string.Empty;
            Token = string.Empty;
        }

        public string SessionId { get; set; }
        public string Token { get; set; }
    }
}