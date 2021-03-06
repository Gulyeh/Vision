namespace VisionClient.Core.Dtos
{
    public class AddPaymentMethodDto
    {
        public AddPaymentMethodDto()
        {
            Photo = string.Empty;
            Provider = string.Empty;
        }

        public string Photo { get; set; }
        public bool IsAvailable { get; set; }
        public string Provider { get; set; }

        public bool Validator() => !string.IsNullOrEmpty(Provider);
    }
}
