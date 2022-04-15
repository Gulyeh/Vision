using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Helpers
{
    public class CreateOrderData
    {
        public CreateOrderData(string productId, Guid userId, string? coupon, string email, string access_Token, OrderType orderType, string? GameId)
        {
            this.productId = Guid.Parse(productId);
            this.userId = userId;
            Coupon = coupon;
            Email = email;
            Access_Token = access_Token;
            this.orderType = orderType;

            Guid _gameId = Guid.Empty;
            Guid.TryParse(GameId, out _gameId);
            gameId = _gameId;
        }

        public Guid productId { get; set; }
        public Guid userId { get; set; }
        public string? Coupon { get; set; }
        public string Email { get; set; }
        public string Access_Token { get; set; }
        public OrderType orderType { get; set; }
        public Guid? gameId { get; set; }
    }
}