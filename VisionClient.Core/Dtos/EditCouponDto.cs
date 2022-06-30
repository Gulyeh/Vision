using VisionClient.Core.Helpers;

namespace VisionClient.Core.Dtos
{
    public class EditCouponDto : NotifyPropertyChanged
    {
        public EditCouponDto()
        {
            Code = string.Empty;
        }

        public Guid Id { get; set; }
        public string Code { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool IsLimited { get; set; }

        private int uses;
        public int Uses
        {
            get => uses;
            set
            {
                if (value <= 0) uses = 1;
                else uses = value;
                OnPropertyChanged();
            }
        }

        public bool Validator() => !string.IsNullOrWhiteSpace(Code) && ExpireDate > DateTime.Now && Id != Guid.Empty;
    }
}
