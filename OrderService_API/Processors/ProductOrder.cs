using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;

namespace OrderService_API.Processors
{
    public class ProductOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;

        public ProductOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender)
        {
            this.rabbitMQSender = rabbitMQSender;
            this.orderRepository = orderRepository;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            return await orderRepository.CreateOrder<ProductDto>(data);
        }

        public OrderType GetOrderType()
        {
            return OrderType.Product;
        }

        public Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order)
        {
            if(rabbitMQSender is null) return Task.FromResult(false);
            rabbitMQSender.SendMessage(new AccessProduct() { userId = data.userId, gameId = order.GameId, productId = order.ProductId, Email = data.Email }, "AccessProductQueue");
            return Task.FromResult(true);
        }
    }
}