using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Processors.Interfaces;
using OrderService_API.RabbitMQSender;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

namespace OrderService_API.Processors
{
    public class CurrencyOrder : IOrder
    {
        private readonly IOrderRepository orderRepository;
        private readonly IRabbitMQSender? rabbitMQSender;
        private readonly IProductsService productsService;

        public CurrencyOrder(IOrderRepository orderRepository, IRabbitMQSender? rabbitMQSender, IProductsService productsService)
        {
            this.productsService = productsService;
            this.orderRepository = orderRepository;
            this.rabbitMQSender = rabbitMQSender;
        }

        public async Task<PaymentMessage?> CreateOrder(CreateOrderData data)
        {
            var productList = await productsService.CheckProductExists<IEnumerable<CurrencyDto>>(data.ProductId, data.Access_Token, data.OrderType, data.GameId);
            if (productList is null) return null;
            var product = productList.FirstOrDefault(x => x.Id == data.ProductId);
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

            var productsList = await productsService.CheckProductExists<IEnumerable<CurrencyDto>>(order.ProductId, data.Access_Token, order.OrderType);
            if (productsList is null) return false;
            var product = productsList.FirstOrDefault(x => x.Id == order.ProductId);
            if (product is null) return false;

            rabbitMQSender.SendMessage(new UpdateFunds() { userId = order.UserId, Amount = product.Amount, Email = data.Email, isCode = false }, "ChangeFundsQueue");
            return true;
        }
    }
}