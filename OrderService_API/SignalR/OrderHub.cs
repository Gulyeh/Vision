using AutoMapper;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender rabbitMQSender;
        private readonly ILogger<OrderHub> logger;
        private readonly IProductsService productsService;
        private readonly IMapper mapper;
        private readonly IOrderTypeProcessor orderTypeProcessor;

        public OrderHub(ICacheService cacheService, IOrderRepository orderRepository, IRabbitMQSender rabbitMQSender,
            ILogger<OrderHub> logger, IProductsService productsService, IMapper mapper, IOrderTypeProcessor orderTypeProcessor)
        {
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.productsService = productsService;
            this.mapper = mapper;
            this.orderTypeProcessor = orderTypeProcessor;
            this.cacheService = cacheService;
        }

        private Guid GetId() => Context.User != null ? Context.User.GetId() : Guid.Empty;
        private string GetEmail() => Context.User != null ? Context.User.GetUsername() : string.Empty;

        public override async Task OnConnectedAsync()
        {
            var userId = GetId();
            var userEmail = GetEmail();
            logger.LogInformation("User with ID: {userId} has connected to OrderHub with ID: {connId}", userId, Context.ConnectionId);

            await cacheService.TryAddToCache(userId, Context.ConnectionId);
            string? token = Context.GetHttpContext()?.Request.Headers["Authorization"][0];
            bool isParsedOrder = Enum.TryParse(Context.GetHttpContext()?.Request.Headers["orderType"], out OrderType order);
            string? productId = Context.GetHttpContext()?.Request.Headers["productId"];
            string? gameId = Context.GetHttpContext()?.Request.Headers["gameId"];
            string? paymentMethodId = Context.GetHttpContext()?.Request.Headers["paymentMethodId"];
            string? coupon = Context.GetHttpContext()?.Request.Headers["couponCode"];

            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(productId) || string.IsNullOrEmpty(paymentMethodId) || !isParsedOrder) throw new HubException("Please provide valid data");
            if (order == OrderType.Product && string.IsNullOrWhiteSpace(gameId)) throw new HubException("Please provide a gameId");

            var orderCreated = orderTypeProcessor.CreateOrder(order);
            if (orderCreated is null) throw new HubException("Wrong orderType");

            var orderData = new CreateOrderData(productId, userId, coupon, userEmail, token, order, gameId, paymentMethodId);
            PaymentMessage? message = await orderCreated.CreateOrder(orderData);

            if (message is not null)
            {
                if (message.TotalPrice > 0) rabbitMQSender.SendMessage(message, "CreatePaymentQueue");
                else rabbitMQSender.SendMessage(mapper.Map<PaymentCompleted>(message), "PaymentQueue");
            }
            else await Clients.Caller.SendAsync("PaymentFailed");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetId();
            await cacheService.TryRemoveFromCache(userId, Context.ConnectionId);
            logger.LogInformation("User with ID: {userId} has disconnected from OrderHub", userId);
        }
    }
}