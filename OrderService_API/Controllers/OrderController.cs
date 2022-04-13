using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService_API.Dtos;
using OrderService_API.Extensions;
using OrderService_API.Repository.IRepository;
using OrderService_API.Statics;

namespace OrderService_API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderRepository orderRepository;

        public OrderController(IOrderRepository orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        [HttpGet("GetOrder")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> GetOrder([FromQuery]Guid orderId){
            if(orderId == Guid.Empty) return BadRequest();
            var order = await orderRepository.GetOrder(orderId);
            if(order is null) return NotFound(new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Order does not exist" }));
            return Ok(new ResponseDto(true, StatusCodes.Status200OK, order));
        }

        [HttpGet("GetOrders")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> GetOrders([FromQuery]Guid productId){
            if(productId == Guid.Empty) return BadRequest();
            return CheckActionResult(await orderRepository.GetOrders(productId));
        }

        [HttpGet("GetUserOrders")]
        public async Task<ActionResult<ResponseDto>> GetUserOrders([FromQuery]Guid productId){
            if(productId == Guid.Empty) return BadRequest();
            var userId = User.GetId();
            return CheckActionResult(await orderRepository.GetOrders(productId, userId));
        }

        [HttpDelete("DeleteOrder")]
        [Authorize(Roles = StaticData.AdminRole)]
        public async Task<ActionResult<ResponseDto>> DeleteOrder([FromQuery]Guid orderId){
            if(orderId == Guid.Empty) return BadRequest();
            return CheckActionResult(await orderRepository.DeleteOrder(orderId));
        }
    }
}