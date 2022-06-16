namespace VisionClient.Core.Models
{
    public class TFADataModel
    {
        public TFADataModel()
        {
            TokenCode = string.Empty;
            QrCodeUri = string.Empty;
        }

        public string TokenCode { get; set; }
        public string QrCodeUri { get; set; }
    }
}
