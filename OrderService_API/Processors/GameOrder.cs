using Newtonsoft.Json;
using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQRPC;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;

namespace OrderService_API.Processors
{
    public class GameOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IRabbitMQRPC rabbitMQRPC;

        public GameOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender, IRabbitMQRPC rabbitMQRPC)
        {
            this.rabbitMQRPC = rabbitMQRPC;
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            var response = await rabbitMQRPC.SendAsync("GetProductsQueue", new GetProductDto(data.ProductId, data.GameId, data.OrderType, data.UserId));
            if (response is null || string.IsNullOrWhiteSpace(response)) return null;

            var product = JsonConvert.DeserializeObject<GameDto>(response);
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