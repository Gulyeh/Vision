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

        public PaymentRepository(ApplicationDbContext db, IMapper mapper, IStripeService stripeService, IRabbitMQSender rabbitMQSender)
        {
            this.mapper = mapper;
            this.stripeService = stripeService;
            this.rabbitMQSender = rabbitMQSender;
            this.db = db;
        }

        public async Task CreatePayment(PaymentMessage data)
        {
            var mapped = mapper.Map<Payment>(data);
            await db.Payments.AddAsync(mapped);
            await db.SaveChangesAsync();
        }

        public async Task<bool> PaymentCompleted(string sessionId, PaymentStatus status, string? Access_Token = null)
        {
            var payment = await db.Payments.FirstOrDefaultAsync(x => x.StripeId == sessionId);
            if (payment is null || payment.PaymentStatus != PaymentStatus.Inprogress) return false;
            payment.PaymentStatus = status;

            var paymentCompletedMessage = new PaymentCompleted();
            paymentCompletedMessage.Email = payment.Email;
            paymentCompletedMessage.userId = payment.UserId;
            paymentCompletedMessage.OrderId = payment.OrderId;
            paymentCompletedMessage.isSuccess = status == PaymentStatus.Completed ? true : false;
            paymentCompletedMessage.Access_Token = Access_Token;
            
            rabbitMQSender.SendMessage(paymentCompletedMessage, "PaymentQueue");
            
            return true;
        }

        public async Task<PaymentUrlData> RequestStripePayment(PaymentMessage data)
        {
            var paymentUrlData = new PaymentUrlData();
            paymentUrlData.userId = data.UserId;
            paymentUrlData.PaymentUrl = await stripeService.GeneratePayment(data);
            return paymentUrlData;
        }
    }
}