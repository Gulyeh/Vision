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

        public OrderRepository(IOrderService orderService)
        {
            this.orderService = orderService;
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
    }
}
