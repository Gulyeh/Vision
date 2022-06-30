namespace VisionClient.Core.Models
{
    public class AttachmentModel : ICloneable
    {
        public AttachmentModel()
        {
            AttachmentUrl = string.Empty;
        }

        public Guid Id { get; set; }
        public string AttachmentUrl { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
