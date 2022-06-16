using VisionClient.Core.Enums;

namespace VisionClient.Core.Models
{
    public class PurchaseModel : BaseProductModel
    {
        public PurchaseModel()
        {
            Title = string.Empty;
            PhotoUrl = string.Empty;
            DetailsList = new List<string>();
            DiscountText = string.Empty;
        }

        public OrderType OrderType { get; set; }
        public Guid Id { get; set; }
        public Guid? GameId { get; set; }
        public string DiscountText { get; private set; }
        public List<string> DetailsList { get; private set; }

        public void SetDetails(string data)
        {
            DetailsList = data.Split("#").ToList();
        }

        public void CreateDiscountText()
        {
            if (Discount > 0) DiscountText = $"-{Discount}%";
        }

        public override void Discounted()
        {
            return;
        }
    }
}
