using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

namespace OrderService_API.Processors
{
    public class ProductOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IProductsService productsService;

        public ProductOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender, IProductsService productsService)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.productsService = productsService;
            this.orderRepository = orderRepository;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            var product = await productsService.CheckProductExists<ProductDto>(data.ProductId, data.Access_Token, data.OrderType, data.GameId);
            if (product is null || !product.IsAvailable || product.IsPurchased) return null;

            return await orderRepository.CreateOrder<ProductDto>(data, product);
        }

        public OrderType GetOrderType()
        {
            return OrderType.Product;
        }

        public Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order)
        {
            if (rabbitMQSender is null) return Task.FromResult(false);
            rabbitMQSender.SendMessage(new AccessProduct(data.UserId, order.GameId, order.ProductId, data.Email), "AccessProductQueue");
            return Task.FromResult(true);
        }
    }
}