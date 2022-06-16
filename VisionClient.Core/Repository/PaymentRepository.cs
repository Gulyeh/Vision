using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IPaymentService paymentService;

        public PaymentRepository(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            var methods = await paymentService.GetPaymentMethods();
            return ResponseToJsonHelper.GetJson<List<PaymentMethod>>(methods);
        }
    }
}
