using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderService orderService;
        private readonly IPaymentService paymentService;

        public OrderRepository(IOrderService orderService, IPaymentService paymentService)
        {
            this.orderService = orderService;
            this.paymentService = paymentService;
        }

        public async Task<(bool, string)> ChangeOrderToPaid(Guid orderId)
        {
            var response = await orderService.ChangeOrderToPaid(orderId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<List<OrderModel>> GetOrders(string? orderId = null)
        {
            var response = await orderService.GetOrders(orderId);
            if(response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson<List<OrderModel>>(response);
        }

        public async Task<List<OrderModel>> GetUserOrders()
        {
            var order = orderService.GetUserOrders();
            var payment = paymentService.GetUserPayments();
            await Task.WhenAll(order, payment);

            var orderResponse = order.Result;
            var paymentResponse = payment.Result;

            if(paymentResponse is null || orderResponse is null) throw new Exception();

            var orderList = ResponseToJsonHelper.GetJson<List<OrderModel>>(orderResponse);
            var paymentList = ResponseToJsonHelper.GetJson<List<PaymentModel>>(paymentResponse);

            foreach(var item in orderList)
            {
                var paymentModel = paymentList.FirstOrDefault(x => x.OrderId == item.Id);
                if (paymentModel is null) continue;

                item.Price = paymentModel.TotalPrice;
                item.PaymentUrl = paymentModel.PaymentUrl;
            }

            return orderList;
        }
    }
}
