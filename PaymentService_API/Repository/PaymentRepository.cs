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
        private readonly IValidateJWT validateJWT;
        private readonly IUploadService uploadService;

        public PaymentRepository(ApplicationDbContext db, IMapper mapper,
            IStripeService stripeService, IRabbitMQSender rabbitMQSender, ILogger<PaymentRepository> logger, ICacheService cacheService,
            IPaymentProcessor paymentProcessor, IValidateJWT validateJWT, IUploadService uploadService)
        {
            this.uploadService = uploadService;
            this.mapper = mapper;
            this.stripeService = stripeService;
            this.rabbitMQSender = rabbitMQSender;
            this.logger = logger;
            this.cacheService = cacheService;
            this.paymentProcessor = paymentProcessor;
            this.validateJWT = validateJWT;
            this.db = db;
        }

        public async Task<ResponseDto> AddPaymentMethod(AddPaymentMethodDto data)
        {
            var mapped = mapper.Map<PaymentMethods>(data);

            var results = await uploadService.UploadPhoto(Convert.FromBase64String(data.Photo));
            mapped.PhotoId = results.PublicId;
            mapped.PhotoUrl = results.SecureUrl.AbsoluteUri;

            if(!string.IsNullOrWhiteSpace(mapped.PhotoId) && !string.IsNullOrWhiteSpace(mapped.PhotoUrl)){
                db.PaymentMethods.Add(mapped);
                if(await db.SaveChangesAsync() > 0){
                    await cacheService.AddMethodsToCache(mapped);
                    logger.LogInformation("New method: {x} - has been added successfully", data.Provider);
                    return new ResponseDto(true, StatusCodes.Status200OK, new[] {"New method has been added successfully"});
                }
            }

            logger.LogInformation("Method: {x} - could not be added", data.Provider);
            return new ResponseDto(false, StatusCodes.Status400BadRequest, new[] {"Method could not be added"});
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

        public async Task<IEnumerable<string>> GetNewProviders()
        {
            var usedProviders = await cacheService.GetMethodsFromCache();
            var usedProvidersName = new List<string>();
            var allProviders = Enum.GetNames(typeof(PaymentProvider));

            foreach(var provider in usedProviders){
                var name = Enum.GetName(typeof(PaymentProvider), provider.Provider);
                if(!string.IsNullOrWhiteSpace(name)) usedProvidersName.Add(name);
            }

            return allProviders.Except(usedProvidersName);
        }

        public async Task<ResponseDto> GetPaymentMethods()
        {
            var methods = await cacheService.GetMethodsFromCache();
            if (methods.Count() == 0)
            {
                var dbMethods = await db.PaymentMethods.ToListAsync();
                foreach (var method in dbMethods) await cacheService.AddMethodsToCache(method);
                methods = dbMethods;
            }
            return new ResponseDto(true, StatusCodes.Status200OK, mapper.Map<IEnumerable<PaymentMethodsDto>>(methods));
        }

        public async Task<ResponseDto> PaymentCompleted(string sessionId, PaymentStatus status, string Access_Token)
        {
            var payment = await db.Payments.FirstOrDefaultAsync(x => x.StripeId == sessionId);
            if (payment is null || payment.PaymentStatus != PaymentStatus.Inprogress) return new ResponseDto(false, StatusCodes.Status400BadRequest, new { });
            payment.PaymentStatus = status;
            await db.SaveChangesAsync();

            var paymentCompletedMessage = new PaymentCompleted();
            paymentCompletedMessage.Email = payment.Email;
            paymentCompletedMessage.userId = payment.UserId;
            paymentCompletedMessage.OrderId = payment.OrderId;
            paymentCompletedMessage.PaymentId = payment.Id;
            paymentCompletedMessage.isSuccess = status == PaymentStatus.Completed ? true : false;
            paymentCompletedMessage.Access_Token = Access_Token;

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
                data.Access_Token = validateJWT.EncodeToken(data.Access_Token);
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
    }
}