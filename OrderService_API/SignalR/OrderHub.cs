using Microsoft.AspNetCore.SignalR;
using OrderService_API.Extensions;
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
            var token = Context.GetHttpContext()?.Request.Query["access_token"];

            Guid gameId;
            Guid.TryParse(Context.GetHttpContext()?.Request.Query["gameId"], out gameId);

            Guid productId;
            Guid.TryParse(Context.GetHttpContext()?.Request.Query["productId"], out productId);

            if (string.IsNullOrEmpty(token) || gameId == Guid.Empty) throw new HubException("Please provide valid token and GameId");
            var paymentMessage = await orderRepository.CreateOrder(gameId, userId, userEmail, token, productId);

            if (paymentMessage is not null) rabbitMQSender.SendMessage(paymentMessage, "CreatePaymentQueue");
            else await Clients.Caller.SendAsync("PaymentDone", new { isSuccess = false });
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await cacheService.TryRemoveFromCache(userId, Context.ConnectionId);
        }
    }
}