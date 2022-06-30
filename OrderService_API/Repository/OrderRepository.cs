using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderService_API.Builders;
using OrderService_API.DbContexts;
using OrderService_API.Dtos;
using OrderService_API.Entities;
using OrderService_API.Helpers;
using OrderService_API.Messages;
using OrderService_API.RabbitMQRPC;
using OrderService_API.Repository.IRepository;

namespace OrderService_API.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IMapper mapper;
        private readonly ApplicationDbContext db;
        private readonly ILogger<OrderRepository> logger;
        private readonly IRabbitMQRPC rabbitMQRPC;

        public OrderRepository(IMapper mapper, ApplicationDbContext db, ILogger<OrderRepository> logger, IRabbitMQRPC rabbitMQRPC)
        {
            this.rabbitMQRPC = rabbitMQRPC;
            this.mapper = mapper;
            this.db = db;
            this.logger = logger;
        }

        public async Task<PaymentMessage?> CreateOrder<T>(CreateOrderData data, T product) where T : BaseProductData
        {
            CouponDataDto couponDiscount = new();

            if (data.OrderType != OrderType.Currency)
            {
                var response = await rabbitMQRPC.SendAsync("CheckProductAccessQueue", new CheckProductAccessDto(data.UserId, data.ProductId, data.GameId));
                if (string.IsNullOrWhiteSpace(response) || bool.Parse(response)) return null;
            }

            if (!string.IsNullOrEmpty(data.Coupon))
            {
                var responseCoupon = await rabbitMQRPC.SendAsync("ApplyCouponQueue", new ApplyCouponDto(data.Coupon, CodeTypes.Discount, data.UserId));
                if (!string.IsNullOrWhiteSpace(responseCoupon) && responseCoupon is not null)
                {
                    var json = JsonConvert.DeserializeObject<CouponDataDto>(responseCoupon);
                    if (json is not null) couponDiscount = json;
                }
            }

            var newOrder = mapper.Map<Order>(data);
            newOrder.Title = product.Title;
            newOrder.CouponCode = data.Coupon;

            var messageBuilder = new PaymentMessageBuilder(product.Price, product.Discount, couponDiscount);
            messageBuilder.SetEmail(data.Email);
            messageBuilder.SetPaymentMethodId(data.PaymentMethodId);
            messageBuilder.SetTitle(product.Title);
            messageBuilder.SetUserId(data.UserId);

            await db.Orders.AddAsync(newOrder);
            if (await SaveChangesAsync())
            {
                messageBuilder.SetOrderId(newOrder.Id);
                logger.LogInformation("User with ID: {userId} has created Order with ID: {orderId}", data.UserId, newOrder.Id);
                return messageBuilder.Build();
            }
            logger.LogError("Could not create an order for User with ID: {userId}", data.UserId);
            return null;
        }

        public async Task<OrderDto> GetOrder(Guid orderId)
        {
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null) return new OrderDto();
            return mapper.Map<OrderDto>(order);
        }

        public async Task<ResponseDto> GetUserOrders(Guid userId)
        {
            var orders = await db.Orders.Where(x => x.UserId == userId).ToListAsync();
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<GetOrdersDto>>(orders));
        }

        public async Task<bool> ChangeOrderStatus(Guid orderId, bool isPaid, Guid? paymentId = null)
        {
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is not null)
            {
                order.Paid = isPaid;
                if (paymentId is not null && paymentId != Guid.Empty) order.PaymentId = paymentId;
                order.PaymentDate = DateTime.Now;
                if (await SaveChangesAsync()) return true;
            }
            return false;
        }

        private async Task<bool> SaveChangesAsync()
        {
            if (await db.SaveChangesAsync() > 0) return true;
            return false;
        }

        public async Task<ResponseDto> GetOrders(string orderId)
        {
            var order = await db.Orders.Where(x => x.Id.ToString().Contains(orderId)).ToListAsync();
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<GetOrdersDto>>(order));
        }
    }
}