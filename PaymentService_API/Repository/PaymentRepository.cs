using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PaymentService_API.DbContexts;
using PaymentService_API.Dtos;
using PaymentService_API.Entities;
using PaymentService_API.Helpers;
using PaymentService_API.Messages;
using PaymentService_API.Processors.Interfaces;
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
        private readonly ILogger<PaymentRepository> logger;
        private readonly ICacheService cacheService;
        private readonly IPaymentProcessor paymentProcessor;
        private readonly IUploadService uploadService;

        public PaymentRepository(ApplicationDbContext db, IMapper mapper,
            IStripeService stripeService, IRabbitMQSender rabbitMQSender, ILogger<PaymentRepository> logger, ICacheService cacheService,
            IPaymentProcessor paymentProcessor, IUploadService uploadService)
        {
            this.uploadService = uploadService;
            this.mapper = mapper;
            this.stripeService = stripeService;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.cacheService = cacheService;
            this.paymentProcessor = paymentProcessor;
            this.db = db;
        }

        public async Task<ResponseDto> AddPaymentMethod(AddPaymentMethodDto data)
        {
            var isProviderParsed = Enum.TryParse(data.Provider, out PaymentProvider providerParsed);
            if (!isProviderParsed) return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Provider does not exist" });

            var mapped = mapper.Map<PaymentMethods>(data);
            mapped.Provider = providerParsed;

            var results = await uploadService.UploadPhoto(data.Photo);
            mapped.PhotoId = results.PublicId;
            mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;

            if (!string.IsNullOrWhiteSpace(mapped.PhotoId) && !string.IsNullOrWhiteSpace(mapped.PhotoUrl))
            {
                db.PaymentMethods.Add(mapped);
                if (await db.SaveChangesAsync() > 0)
                {
                    await cacheService.AddMethodsToCache(mapped);
                    logger.LogInformation("New method: {x} - has been added successfully", data.Provider);
                    return new ResponseDto(true, StatusCodes.Status200OK, new[] { "New method has been added successfully" });
                }
            }

            logger.LogInformation("Method: {x} - could not be added", data.Provider);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Method could not be added" });
        }

        public async Task CreatePayment(PaymentMessage data)
        {
            var mapped = mapper.Map<Payment>(data);
            var providers = await db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == data.PaymentMethodId);
            if (providers is not null)
            {
                var providerName = Enum.GetName(typeof(PaymentProvider), providers.Provider);
                if (!string.IsNullOrEmpty(providerName)) mapped.Provider = providerName;
            }
            await db.Payments.AddAsync(mapped);
            await db.SaveChangesAsync();
            logger.LogInformation("Created Payment for Order with ID: {orderId}", data.OrderId);
        }

        public async Task<ResponseDto> DeletePaymentMethod(Guid paymentId)
        {
            var paymentMethod = await db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == paymentId);
            if (paymentMethod is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Payment method does not exist" });

            db.PaymentMethods.Remove(paymentMethod);
            if (await db.SaveChangesAsync() > 0)
            {
                var cached = await cacheService.GetMethodsFromCache();
                if (cached is not null && cached.Any())
                {
                    var method = cached.FirstOrDefault(x => x.Id == paymentId);
                    if (method is not null) await cacheService.RemoveMethodsFromCache(method);
                }

                await uploadService.DeletePhoto(paymentMethod.PhotoId);

                logger.LogInformation("Payment method with ID: {x} - has been deleted", paymentId);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Payment method has been deleted" });
            }

            logger.LogInformation("Payment method with ID: {x} - could not be deleted", paymentId);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Payment method could not be deleted" });
        }

        public async Task<IEnumerable<string>> GetNewProviders()
        {
            var usedProviders = await cacheService.GetMethodsFromCache();
            var usedProvidersName = new List<string>();
            var allProviders = Enum.GetNames(typeof(PaymentProvider));

            foreach (var provider in usedProviders)
            {
                var name = Enum.GetName(typeof(PaymentProvider), provider.Provider);
                if (!string.IsNullOrWhiteSpace(name)) usedProvidersName.Add(name);
            }

            return allProviders.Except(usedProvidersName);
        }

        public async Task<ResponseDto> GetPaymentMethods() => new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<PaymentMethodsDto>>(await cacheService.GetMethodsFromCache()));

        public async Task<ResponseDto> GetUserPayments(Guid userId)
        {
            var payments = await db.Payments.Where(x => x.UserId == userId).ToListAsync();
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<GetPaymentsDto>>(payments));
        }

        public async Task<ResponseDto> PaymentCompleted(string sessionId, PaymentStatus status)
        {
            var payment = await db.Payments.FirstOrDefaultAsync(x => x.PaymentId == sessionId);
            if (payment is null || payment.PaymentStatus != PaymentStatus.Inprogress) return new ResponseDto(false, StatusCodes.Status400BadRequest, new { });
            payment.PaymentStatus = status;
            await db.SaveChangesAsync();

            var paymentCompletedMessage = new PaymentCompleted();
            paymentCompletedMessage.Email = payment.Email;
            paymentCompletedMessage.userId = payment.UserId;
            paymentCompletedMessage.OrderId = payment.OrderId;
            paymentCompletedMessage.PaymentId = payment.Id;
            paymentCompletedMessage.isSuccess = status == PaymentStatus.Completed ? true : false;

            rabbitMQSender.SendMessage(paymentCompletedMessage, "PaymentQueue");

            var paymentStatus = status == PaymentStatus.Cancelled ? "cancelled" : "completed";
            logger.LogInformation($"Payment has been {paymentStatus} for Order with ID: {payment.OrderId}");
            return new ResponseDto(true, StatusCodes.Status200OK, new { orderId = payment.OrderId, paymentId = payment.Id, status = paymentStatus });
        }

        public async Task<PaymentUrlData> RequestPayment(PaymentMessage data)
        {
            var paymentUrlData = new PaymentUrlData();
            paymentUrlData.userId = data.UserId;
            try
            {
                var providers = await db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == data.PaymentMethodId);
                if (providers is null) return paymentUrlData;
                paymentUrlData.PaymentUrl = await paymentProcessor.GetProvider(providers.Provider).GeneratePaymentUrl(data);
                logger.LogInformation("Generated payment Url for Order with ID: {orderId}", data.OrderId);
                return paymentUrlData;
            }
            catch (Exception)
            {
                return paymentUrlData;
            }
        }

        public async Task<ResponseDto> UpdatePaymentMethod(EditPaymentMethodDto data)
        {
            string oldPhotoId = string.Empty;

            var payment = await db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == data.Id);
            if (payment is null) return new ResponseDto(false, StatusCodes.Status404NotFound, new[] { "Payment method does not exist" });

            mapper.Map(data, payment);

            if (!string.IsNullOrWhiteSpace(data.Photo))
            {
                var results = await uploadService.UploadPhoto(data.Photo);
                oldPhotoId = payment.PhotoId;
                payment.PhotoId = results.PublicId;
                payment.PhotoUrl = results.SecureUrl.AbsoluteUri;
            }

            if (await db.SaveChangesAsync() > 0)
            {
                if (!string.IsNullOrWhiteSpace(oldPhotoId)) await uploadService.DeletePhoto(oldPhotoId);
                await cacheService.ReplacePaymentMethod(payment);

                logger.LogInformation("Payment method with ID: {x} - has been edited successfully", data.Id);
                return new ResponseDto(true, StatusCodes.Status200OK, new[] { "Payment method has been edited successfully" });
            }

            logger.LogInformation("Method: {x} - could not be edited", data.Id);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] { "Method could not be edited" });
        }
    }
}