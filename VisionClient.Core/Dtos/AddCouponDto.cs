using VisionClient.Core.Helpers;

namespace VisionClient.Core.Dtos
{
    public class AddCouponDto : NotifyPropertyChanged
    {
        public AddCouponDto()
        {
            Code = string.Empty;
            codeValue = string.Empty;
            codeType = string.Empty;
            signature = string.Empty;
        }

        public string Code { get; set; }
        public DateTime ExpireDate { get; set; } = DateTime.Now;
        public Guid GameId { get; set; }

        public bool IsLimited { get; set; }

        private string codeValue;
        public string CodeValue
        {
            get => codeValue;
            set
            {
                if (Signature.Equals("Procentage"))
                {
                    var isParsed = int.TryParse(value, out int parsed);
                    if (isParsed)
                    {
                        if (parsed > 100) codeValue = string.Empty;
                        else codeValue = value;
                    }
                }
                else codeValue = value;
                OnPropertyChanged();
            }
        }

        private string signature;
        public string Signature
        {
            get => signature;
            set
            {
                signature = value;
                CodeValue = string.Empty;
                OnPropertyChanged(nameof(CodeValue));
            }
        }

        private string codeType;
        public string CodeType
        {
            get => codeType;
            set
            {
                codeType = value;
                ResetProperties();
            }
        }
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


        private void ResetProperties()
        {
            CodeValue = string.Empty;
            Signature = string.Empty;
            GameId = Guid.Empty;
        }

        public bool Validator()
        {
            if (string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(CodeValue) || ExpireDate <= DateTime.Now || (IsLimited && Uses < 1)) return false;

            switch (CodeType)
            {
                case "Discount":
                    return !string.IsNullOrWhiteSpace(Signature);
                case "Package":
                    return GameId != Guid.Empty;
                default:
                    break;
            }

            return true;
        }
    }
}
