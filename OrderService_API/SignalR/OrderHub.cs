using Microsoft.AspNetCore.SignalR;
using OrderService_API.Dtos;
using OrderService_API.Extensions;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors;
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
        private readonly ILogger<OrderHub> logger;

        public OrderHub(ICacheService cacheService, IOrderRepository orderRepository, IRabbitMQSender rabbitMQSender,
            ILogger<OrderHub> logger)
        {
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.cacheService = cacheService;
            userId = Context.User != null ? Context.User.GetId() : Guid.Empty;
            userEmail = Context.User != null ? Context.User.GetUsername() : string.Empty;
        }

        public override async Task OnConnectedAsync()
        {
            await cacheService.TryAddToCache(userId, Context.ConnectionId);
            string? token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
            Enum.TryParse(Context.GetHttpContext()?.Request.Query["orderType"], out OrderType order);
            string? productId = Context.GetHttpContext()?.Request.Query["productId"];
            string? gameId = Context.GetHttpContext()?.Request.Query["gameId"];
            string? coupon = Context.GetHttpContext()?.Request.Query["couponCode"];

            if(string.IsNullOrEmpty(token) || string.IsNullOrEmpty(productId)) throw new HubException("Please provide valid data");

            var orderCreated = new OrderTypeProcessor(order, orderRepository).CreateOrder();
            if(orderCreated is null) throw new HubException("Wrong orderType");
            var orderType = orderCreated.GetOrderType();

            if(orderType == OrderType.Game && string.IsNullOrEmpty(gameId)) throw new HubException("Please provide a gameId");

            var orderData = new CreateOrderData(productId, userId, coupon, userEmail, token, orderType, gameId);
            PaymentMessage? message = await orderCreated.CreateOrder(orderData);

            if (message is not null) rabbitMQSender.SendMessage(message, "CreatePaymentQueue");
            else await Clients.Caller.SendAsync("PaymentDone", new { isSuccess = false });

            logger.LogInformation("User with ID: {userId} has connected to OrderHub with ID: {connId}", userId, Context.ConnectionId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await cacheService.TryRemoveFromCache(userId, Context.ConnectionId);
            logger.LogInformation("User with ID: {userId} has disconnected from OrderHub", userId);
        }
    }
}