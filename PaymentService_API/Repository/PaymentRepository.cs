using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentService_API.DbContexts;
using PaymentService_API.Entities;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;
using PaymentService_API.RabbitMQSender;
using PaymentService_API.Repository.IRepository;
using PaymentService_API.Services.IServices;

namespace PaymentService_API.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly IStripeService stripeService;
        private readonly IRabbitMQSender rabbitMQSender;

        public PaymentRepository(DbContextOptions<ApplicationDbContext> db, IMapper mapper, IStripeService stripeService, IRabbitMQSender rabbitMQSender)
        {
            this.mapper = mapper;
            this.stripeService = stripeService;
            this.rabbitMQSender = rabbitMQSender;
            this.db = new ApplicationDbContext(db);
        }

        public async Task CreatePayment(PaymentMessage data)
        {
            var mapped = mapper.Map<Payment>(data);
            await db.Payments.AddAsync(mapped);
            await db.SaveChangesAsync();
        }

        public async Task<bool> PaymentCompleted(string sessionId, PaymentStatus status)
        {
            var payment = await db.Payments.FirstOrDefaultAsync(x => x.StripeId == sessionId);
            if (payment is null || payment.PaymentStatus != PaymentStatus.Inprogress) return false;
            payment.PaymentStatus = status;

            rabbitMQSender.SendMessage(new
            {
                isSuccess = status == PaymentStatus.Completed ? true : false,
                userId = payment.UserId,
                Email = payment.Email,
                orderId = payment.OrderId
            }, "PaymentQueue");
            
            return true;
        }

        public async Task<PaymentUrlData> RequestStripePayment(PaymentMessage data)
        {
            var url = await stripeService.GeneratePayment(data);
            return new PaymentUrlData()
            {
                userId = data.UserId,
                PaymentUrl = url
            };
        }
    }
}