using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService_API.Helpers
{
    public class CreateOrderData
    {
        public CreateOrderData(string productId, Guid userId, string? coupon, string email, string access_Token, OrderType orderType, string? gameId)
        {
            ProductId = Guid.Parse(productId);
            UserId = userId;
            Coupon = coupon;
            Email = email;
            Access_Token = access_Token;
            OrderType = orderType;

            Guid _gameId = Guid.Empty;
            Guid.TryParse(gameId, out _gameId);
            GameId = _gameId;
        }

        public Guid ProductId { get; private set; }
        public Guid UserId { get; private set; }
        public string? Coupon { get; private set; }
        public string Email { get; private set; }
        public string Access_Token { get; private set; }
        public OrderType OrderType { get; private set; }
        public Guid? GameId { get; private set; }
    }
}