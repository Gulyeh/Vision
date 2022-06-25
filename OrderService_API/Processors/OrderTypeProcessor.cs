using OrderService_API.Helpers;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

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
        private readonly IProductsService productsService;

        public OrderTypeProcessor(IOrderRepository orderRepository, IProductsService productsService, IRabbitMQSender rabbitMQSender)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.productsService = productsService;
            this.orderRepository = orderRepository;
        }

        public IOrder? GetOrderOfType(OrderType orderType)
        {
            return orderType switch
            {
                OrderType.Currency => new CurrencyOrder(orderRepository, rabbitMQSender, productsService),
                OrderType.Game => new GameOrder(orderRepository, rabbitMQSender, productsService),
                OrderType.Product => new ProductOrder(orderRepository, rabbitMQSender, productsService),
                _ => null
            };
        }

    }
}