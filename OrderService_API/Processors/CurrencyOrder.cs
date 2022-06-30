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
    public class CurrencyOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IRabbitMQRPC rabbitMQRPC;

        public CurrencyOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender, IRabbitMQRPC rabbitMQRPC)
        {
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
            this.rabbitMQRPC = rabbitMQRPC;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            var response = await rabbitMQRPC.SendAsync("GetProductsQueue", new GetProductDto(data.ProductId, data.GameId, data.OrderType, data.UserId));
            if (response is null || string.IsNullOrWhiteSpace(response)) return null;

            var productList = JsonConvert.DeserializeObject<IEnumerable<CurrencyDto>>(response);
            var product = productList?.FirstOrDefault(x => x.Id == data.ProductId);
            if (product is null || !product.IsAvailable) return null;

            return await orderRepository.CreateOrder<CurrencyDto>(data, product);
        }

        public OrderType GetOrderType()
        {
            return OrderType.Currency;
        }

        public async Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order)
        {
            if (rabbitMQSender is null) return false;

            var response = await rabbitMQRPC.SendAsync("GetProductsQueue", new GetProductDto(order.ProductId, order.GameId, order.OrderType, data.UserId));
            if (response is null || string.IsNullOrWhiteSpace(response)) return false;

            var productList = JsonConvert.DeserializeObject<IEnumerable<CurrencyDto>>(response);
            var product = productList?.FirstOrDefault(x => x.Id == order.ProductId);
            if (product is null || !product.IsAvailable) return false;

            rabbitMQSender.SendMessage(new UpdateFunds() { userId = order.UserId, Amount = product.Amount, Email = data.Email, isCode = false }, "ChangeFundsQueue");
            return true;
        }
    }
}