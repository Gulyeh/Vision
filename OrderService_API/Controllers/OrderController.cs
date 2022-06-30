using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService_API.Dtos;
using OrderService_API.Extensions;
using OrderService_API.Repository.IRepository;

namespace OrderService_API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderRepository orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        [HttpGet("GetOrders")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> GetOrders([FromQuery] string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) return BadRequest();
            return Ok(await orderRepository.GetOrders(orderId));
        }

        [HttpGet("GetUserOrders")]
        [Authorize]
        public async Task<ActionResult<ResponseDto>> GetUserOrders()
        {
            var userId = User.GetId();
            return CheckActionResult(await orderRepository.GetUserOrders(userId));
        }

        [HttpPost("ChangeToPaid")]
        [Authorize(Policy = "HasAdminRole")]
        public async Task<ActionResult<ResponseDto>> ChangeToPaid([FromQuery] Guid orderId)
        {
            if (orderId == Guid.Empty) return BadRequest();
            if (!await orderRepository.ChangeOrderStatus(orderId, true)) return CheckActionResult(new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not change status" }));
            return CheckActionResult(new ResponseDto(true, StatusCodes.Status200OK, new[] { "Status has been changed" }));
        }
    }
}