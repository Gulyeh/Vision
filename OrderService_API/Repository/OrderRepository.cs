using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService_API.DbContexts;
using OrderService_API.Dtos;
using OrderService_API.Entities;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.Repository.IRepository;
using OrderService_API.Services.IServices;

namespace OrderService_API.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;
        private readonly IProductsService productsService;
        private readonly ICouponService couponService;
        private readonly ILogger<OrderRepository> logger;

        public OrderRepository(IMapper mapper, ApplicationDbContext db, IProductsService productsService, 
            ICouponService couponService, ILogger<OrderRepository> logger)
        {
            this.mapper = mapper;
            this.db = db;
            this.productsService = productsService;
            this.couponService = couponService;
            this.logger = logger;
        }

        public async Task<PaymentMessage?> CreateOrder<T>(CreateOrderData data) where T : BaseProductData
        {
            var product = await productsService.CheckProductExists<T>(data.ProductId, data.Access_Token, data.OrderType, data.GameId);
            if (product is null) return null;

            int couponDiscount = 0;
            if(!string.IsNullOrEmpty(data.Coupon)) couponDiscount = await couponService.ApplyCoupon(data.Coupon, data.Access_Token, CodeTypes.Discount);

            var newOrder = mapper.Map<Order>(data);
            newOrder.CuponCode = couponDiscount != 0 ? data.Coupon : string.Empty;

            var message = new PaymentMessage(product.Price, product.Discount, couponDiscount);
            message.UserId = data.UserId;
            message.Email = data.Email;
            message.Title = product.Title;      
            

            await db.Orders.AddAsync(newOrder);
            if (await SaveChangesAsync())
            {
                message.OrderId = newOrder.Id;
                logger.LogInformation("User with ID: {userId} has created Order with ID: {orderId}", data.UserId, newOrder.Id);
                return message;
            }
            logger.LogError("Could not create an order for User with ID: {userId}", data.UserId);
            return null;
        }

        public async Task<ResponseDto> DeleteOrder(Guid orderId)
        {
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Order not found" });

            db.Orders.Remove(order);
            if (await SaveChangesAsync()) {
                logger.LogInformation("Deleted Order with Id: {orderId} successfully", orderId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Deleted order successfuly" });
            }
            logger.LogInformation("Could not delete an Order with ID: {orderId}", orderId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Could not delete order" });
        }

        public async Task<OrderDto> GetOrder(Guid orderId)
        {
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null) return new OrderDto();
            return mapper.Map<OrderDto>(order);
        }

        public async Task<ResponseDto> GetOrders(Guid productId, Guid? userId = null)
        {
            IEnumerable<Order> order;
            if (userId is null) order = await db.Orders.Where(x => x.ProductId == productId).ToListAsync();
            else order = await db.Orders.Where(x => x.ProductId == productId && x.UserId == userId).ToListAsync();

            if (order is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Could not find orders" });

            var mapped = mapper.Map<IEnumerable<OrderDto>>(order);
            return new ResponseDto(true, StatusCodes.Status200OK, mapped);
        }

        public async Task ChangeOrderStatus(Guid orderId, bool isPaid)
        {
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is not null)
            {
                order.Paid = isPaid;
                order.PaymentDate = DateTime.UtcNow;
                await SaveChangesAsync();
            }
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }
    }
}