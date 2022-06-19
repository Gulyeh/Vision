using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamesDataService_API.Messages;

namespace GamesDataService_API.Builder
{
    public class ProductBuilder
    {
        NewProductDto product = new();

        public NewProductDto Build() => product;
        public void SetTitle(string title) => product.Title = title;
        public void SetGameId(Guid GameId) => product.GameId = GameId;
        public void SetPhotoUrl(string url) => product.PhotoUrl = url;
        public void SetPhotoId(string Id) => product.PhotoId = Id;
        public void SetPrice(decimal price) => product.Price = price;
        public void SetIsAvailable(bool IsAvailable) => product.IsAvailable = IsAvailable;
        public void SetDiscount(int discount) => product.Discount = discount;
        public void SetDetails(string details) => product.Details = details;
    }
}