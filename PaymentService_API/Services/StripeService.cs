using Microsoft.EntityFrameworkCore;
using PaymentService_API.DbContexts;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;
using PaymentService_API.Services.IServices;
using Stripe;
using Stripe.Checkout;

namespace PaymentService_API.Services
{
    public class StripeService : IStripeService
    {
        private readonly ApplicationDbContext db;
        public readonly string WebclientAddress;

        public StripeService(ApplicationDbContext db, IConfiguration config)
        {
            this.db = db;
            WebclientAddress = config.GetValue<string>("WebclientAddress");
        }

        private Task<string> CheckCustomer(string email, Guid userId)
        {
            var optionsSearch = new CustomerSearchOptions
            {
                Query = $"email:'{email}'",
            };
            var serviceSearch = new CustomerService();
            var list = serviceSearch.Search(optionsSearch);
            if (list.Any()) return Task.FromResult(list.First().Id);

            var options = new CustomerCreateOptions
            {
                Email = email,
                Description = $"ID: {userId}"
            };
            var service = new CustomerService();
            var created = service.Create(options);
            if (created is not null && created.Equals(email)) return Task.FromResult(created.Id);
            return Task.FromResult(string.Empty);
        }

        public async Task<string> GeneratePayment(PaymentMessage data)
        {
            var CustomerId = await CheckCustomer(data.Email, data.UserId);
            if (string.IsNullOrEmpty(CustomerId)) return string.Empty;

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions{
                        UnitAmountDecimal = data.TotalPrice*100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions{
                            Name = data.Title
                        }
                    },
                    Quantity = 1
                  },
                },
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "payment",
                SuccessUrl = $"{WebclientAddress}/payment/success?sessionId=" + "{CHECKOUT_SESSION_ID}" + $"&token={data.Access_Token}",
                CancelUrl = $"{WebclientAddress}/payment/failed?sessionId=" + "{CHECKOUT_SESSION_ID}",
                ExpiresAt = DateTime.Now + new TimeSpan(0, 60, 0),
                Customer = CustomerId,
            };
            var service = new SessionService();
            Session session = service.Create(options);

            var payment = await db.Payments.FirstOrDefaultAsync(x => x.OrderId == data.OrderId);
            if (payment is not null)
            {
                payment.PaymentUrl= session.Url;
                payment.PaymentStatus = PaymentStatus.Inprogress;
                payment.PaymentId = session.Id;
                await db.SaveChangesAsync();
            }

            return session.Url;
        }
    }
}