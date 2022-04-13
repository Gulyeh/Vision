using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OrderService_API.DbContexts;
using OrderService_API.Dtos;
using OrderService_API.Entities;
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

        public OrderRepository(IMapper mapper, ApplicationDbContext db, IProductsService productsService)
        {
            this.mapper = mapper;
            this.db = db;
            this.productsService = productsService;
        }

        public async Task<PaymentMessage?> CreateOrder(Guid gameId, Guid userId, string Email, string Access_Token, Guid? productId = null)
        {
            if (!await productsService.CheckProductExists<bool>(gameId, Access_Token, productId)) return null;


            var newOrder = new Order()
            {
                GameId = gameId,
                ProductId = productId,
                UserId = userId
            };

            var game = await productsService.GetGame(newOrder.GameId, Access_Token);
            var message = new PaymentMessage();
            message.UserId = userId;
            message.Email = Email;

            if (productId is not null)
            {
                var product = game.GameProducts?.FirstOrDefault(x => x.ProductId == productId);
                if (product is not null)
                {
                    message.TotalPrice = product.Price;
                    message.Title = product.Title;
                }
            }
            else
            {
                message.TotalPrice = game.Price;
                message.Title = game.Title;
            }

            await db.Orders.AddAsync(newOrder);
            if (await SaveChangesAsync())
            {
                message.OrderId = newOrder.Id;
                return message;
            }
            return null;
        }

        public async Task<ResponseDto> DeleteOrder(Guid orderId)
        {
            var order = await db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Order not found" });

            db.Orders.Remove(order);
            if (await SaveChangesAsync()) return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Deleted order successfuly" });

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
            if (userId is null)
            {
                order = await db.Orders.Where(x => x.ProductId == productId).ToListAsync();
            }
            else
            {
                order = await db.Orders.Where(x => x.ProductId == productId && x.UserId == userId).ToListAsync();
            }

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