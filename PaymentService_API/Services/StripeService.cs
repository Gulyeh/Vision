using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentService_API.DbContexts;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;
using PaymentService_API.Services.IServices;
using Stripe.Checkout;

namespace PaymentService_API.Services
{
    public class StripeService : IStripeService
    {
        private readonly ApplicationDbContext db;

        public StripeService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public async Task<string> GeneratePayment(PaymentMessage data)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    Price = data.TotalPrice.ToString(),
                    Quantity = 1
                  },
                },
                PaymentMethodTypes = new List<string> 
                {
                    "card",
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7271/payment/success?sessionId={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7271/payment/failed?sessionId={CHECKOUT_SESSION_ID}",
                ExpiresAt = DateTime.UtcNow + new TimeSpan(0, 10, 0),
                Customer = data.UserId.ToString(),
            };
            var service = new SessionService();
            Session session = service.Create(options);

            var payment = await db.Payments.FirstOrDefaultAsync(x => x.OrderId == data.OrderId);
            if(payment is not null){
                payment.StripeUrl = session.Url;
                payment.PaymentStatus = PaymentStatus.Inprogress;
                payment.StripeId = session.Id;
                await db.SaveChangesAsync();
            }

            return session.Url;
        }
    }
}