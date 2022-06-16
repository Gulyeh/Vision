using OrderService_API.Dtos;
using OrderService_API.Helpers;
using OrderService_API.Messages;

namespace OrderService_API.Processors.Interfaces
{
    public interface IOrder
    {
        OrderType GetOrderType();
        Task<PaymentMessage?> CreateOrder(CreateOrderData data);
        Task<bool> PaymentCompleted(PaymentCompleted data, OrderDto order);
    }
}