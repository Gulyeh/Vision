namespace VisionClient.Core.Models
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            PhotoUrl = string.Empty;
        }

        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
