using Microsoft.AspNetCore.SignalR;
using OrderService_API.Dtos;
using OrderService_API.Extensions;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

namespace OrderService_API.SignalR
{
    public class OrderHub : Hub
    {
        private readonly ICacheService cacheService;
        private readonly Guid userId;
        private readonly string userEmail;
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender rabbitMQSender;

        public OrderHub(ICacheService cacheService, IOrderRepository orderRepository, IRabbitMQSender rabbitMQSender)
        {
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
            this.cacheService = cacheService;
            userId = Context.User != null ? Context.User.GetId() : Guid.Empty;
            userEmail = Context.User != null ? Context.User.GetUsername() : string.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            await cacheService.TryAddToCache(userId, Context.ConnectionId);
            string? token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
            string? order = Context.GetHttpContext()?.Request.Query["orderType"];
            string? productId = Context.GetHttpContext()?.Request.Query["productId"];
            string? gameId = Context.GetHttpContext()?.Request.Query["gameId"];
            string? coupon = Context.GetHttpContext()?.Request.Query["couponCode"];

            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(productId)) throw new HubException("Please provide a token and productId");

            OrderType orderType = order switch{
                "currency" => OrderType.Currency,
                "game" => OrderType.Game,
                "product" => OrderType.Product,
                _ => throw new HubException("Wrong orderType")
            };  

            if(orderType == OrderType.Game && string.IsNullOrEmpty(gameId)) throw new HubException("Please provide a gameId");

            var orderData = new CreateOrderData(productId, userId, coupon, userEmail, token, orderType, gameId);
            PaymentMessage? message = null;

            _ = orderType switch
            {
                OrderType.Currency => message = await orderRepository.CreateOrder<CurrencyDto>(orderData),
                OrderType.Game => message = await orderRepository.CreateOrder<GameDto>(orderData),
                OrderType.Product => message = await orderRepository.CreateOrder<ProductDto>(orderData),
                _ => null
            };

            if (message is not null) rabbitMQSender.SendMessage(message, "CreatePaymentQueue");
            else await Clients.Caller.SendAsync("PaymentDone", new { isSuccess = false });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await cacheService.TryRemoveFromCache(userId, Context.ConnectionId);
        }
    }
}