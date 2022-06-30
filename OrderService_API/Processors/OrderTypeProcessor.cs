using OrderService_API.Helpers;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQRPC;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;

namespace OrderService_API.Processors
{
    public interface IOrderTypeProcessor
    {
        IOrder? GetOrderOfType(OrderType orderType);
    }

    public class OrderTypeProcessor : IOrderTypeProcessor
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IRabbitMQRPC rabbitMQRPC;

        public OrderTypeProcessor(IOrderRepository orderRepository, IRabbitMQSender rabbitMQSender, IRabbitMQRPC rabbitMQRPC)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.rabbitMQRPC = rabbitMQRPC;
            this.orderRepository = orderRepository;
        }

        public IOrder? GetOrderOfType(OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Currency => new CurrencyOrder(orderRepository, rabbitMQSender, rabbitMQRPC),
                OrderType.Game => new GameOrder(orderRepository, rabbitMQSender, rabbitMQRPC),
                OrderType.Product => new ProductOrder(orderRepository, rabbitMQSender, rabbitMQRPC),
                _ => null
            };
        }

    }
}