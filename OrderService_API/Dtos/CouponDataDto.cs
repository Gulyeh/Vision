namespace OrderService_API.Dtos
{
    public class CouponDataDto
    {
        public CouponDataDto()
        {
            Signature = string.Empty;
        }

        private int codeValue = 0;
        public int CodeValue
        {
            get => codeValue;
            set
            {
                var isParsed = int.TryParse(value.ToString(), out int ParsedValue);
                if (isParsed) codeValue = ParsedValue;
                else codeValue = 0;
            }
        }
        public string Signature { get; set; }
    }
}