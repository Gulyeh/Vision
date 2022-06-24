using PaymentService_API.Helpers;

namespace PaymentService_API.Dtos
{
    public class PaymentMethodsDto
    {
        public PaymentMethodsDto()
        {
            PhotoUrl = string.Empty;
            title = string.Empty;
        }

        public Guid Id { get; set; }
        public string PhotoUrl { get; set; }
        private string title;
        public string Title 
        { 
            get => title;
            set {
                var parseInt = int.TryParse(value, out int EnumValue);
                if(!parseInt){
                    title = value;
                    return;
                }

                var parsedEnum = Enum.GetName(typeof(PaymentProvider), parseInt);
                if(!string.IsNullOrEmpty(parsedEnum)) title = parsedEnum;
            }
        }
        public bool IsAvailable { get; set; }
    }
}