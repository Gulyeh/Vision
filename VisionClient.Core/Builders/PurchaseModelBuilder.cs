using VisionClient.Core.Enums;
using VisionClient.Core.Models;

namespace VisionClient.Core.Builders
{
    public class PurchaseModelBuilder
    {
        private readonly PurchaseModel data = new();

        public PurchaseModel Build()
        {
            data.CreateDiscountText();
            return data;
        }

        public void SetOrderType(OrderType orderType)
        {
            data.OrderType = orderType;
        }

        public void SetGameId(Guid GameId)
        {
            data.GameId = GameId;
        }

        public void SetId(Guid Id)
        {
            data.Id = Id;
        }

        public void SetDetails(string details)
        {
            data.SetDetails(details);
        }

        public void SetTitle(string title)
        {
            data.Title = title;
        }

        public void SetPhotoUrl(string photoUrl)
        {
            data.PhotoUrl = photoUrl;
        }

        public void SetPrice(decimal price)
        {
            data.Price = price;
        }

        public void SetOldPrice(decimal? oldPrice)
        {
            data.OldPrice = oldPrice;
        }

        public void SetDiscount(int? discount)
        {
            data.Discount = discount;
        }
    }
}
