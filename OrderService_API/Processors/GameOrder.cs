using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

namespace OrderService_API.Processors
{
    public class GameOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IProductsService productsService;

        public GameOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender, IProductsService productsService)
        {
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
            this.productsService = productsService;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            var product = await productsService.CheckProductExists<GameDto>(data.ProductId, data.Access_Token, data.OrderType, data.GameId);
            if (product is null || !product.IsAvailable || product.IsPurchased) return null;

            return await orderRepository.CreateOrder<GameDto>(data, product);
        }

        public OrderType GetOrderType()
        {
            return OrderType.Game;
        }

        public Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order)
        {
            if (rabbitMQSender is null) return Task.FromResult(false);
            rabbitMQSender.SendMessage(new AccessProduct(data.UserId, order.GameId, order.ProductId, data.Email), "AccessProductQueue");
            return Task.FromResult(true);
        }
    }
}