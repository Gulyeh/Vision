using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Helpers;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

namespace OrderService_API.Processors
{
    public class OrderTypeProcessor
    {
        private readonly OrderType orderType;
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IProductsService? productsService;

        public OrderTypeProcessor(OrderType orderType, IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender = null, IProductsService? productsService = null)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.productsService = productsService;
            this.orderType = orderType;
            this.orderRepository = orderRepository;
        }

        public IOrder? CreateOrder(){
            return orderType switch{
                OrderType.Currency => new CurrencyOrder(orderRepository, rabbitMQSender, productsService),
                OrderType.Game => new GameOrder(orderRepository, rabbitMQSender),
                OrderType.Product => new ProductOrder(orderRepository, rabbitMQSender),
                _ => null
            };
        }

    }
}