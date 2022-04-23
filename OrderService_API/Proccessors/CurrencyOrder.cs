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
using OrderService_API.Services.IServices;

namespace OrderService_API.Proccessors
{
    public class CurrencyOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IProductsService? productsService;

        public CurrencyOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender, IProductsService? productsService)
        {
            this.productsService = productsService;
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            return await orderRepository.CreateOrder<CurrencyDto>(data);
        }

        public OrderType GetOrderType()
        {
            return OrderType.Currency;
        }

        public async Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order)
        {
            if(rabbitMQSender is null || productsService is null || string.IsNullOrEmpty(data.Access_Token)) return false;

            var product = await productsService.CheckProductExists<CurrencyDto>(order.ProductId, data.Access_Token, order.OrderType);
            if(product is null) return false;

            rabbitMQSender.SendMessage(new UpdateFunds() { userId = order.UserId, Amount = product.Amount, Email = data.Email, isCode = false }, "ChangeFundsQueue");
            return true;
        }
    }
}