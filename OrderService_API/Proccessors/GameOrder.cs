using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Proccessors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;

namespace OrderService_API.Proccessors
{
    public class GameOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;

        public GameOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender)
        {
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            return await orderRepository.CreateOrder<GameDto>(data);
        }

        public OrderType GetOrderType()
        {
            return OrderType.Game;
        }

        public Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order)
        {
            if(rabbitMQSender is null) return Task.FromResult(false);
            rabbitMQSender.SendMessage(new AccessProduct() { userId = data.userId, gameId = order.GameId, productId = order.ProductId, Email = data.Email }, "AccessProductQueue");
            return Task.FromResult(true);
        }
    }
}