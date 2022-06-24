namespace VisionClient.Core.Models
{
    public class PaymentMethod
    {
        public PaymentMethod()
        {
            PhotoUrl = string.Empty;
            Title = string.Empty;
        }

        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }
        public string Title { get; set; }
        public bool IsAvailable { get; set; }
    }
}
