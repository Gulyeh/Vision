using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IOrderRepository
    {
        Task<List<OrderModel>> GetOrders(string? orderId = null);
        Task<(bool, string)> ChangeOrderToPaid(Guid orderId);
    }
}
