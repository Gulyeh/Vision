using VisionClient.Core.Dtos;
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

        public async Task<(bool, string)> AddPaymentMethod(AddPaymentMethodDto data)
        {
            var response = await paymentService.CreatePaymentMethod(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> DeleteMethod(Guid paymentId)
        {
            var response = await paymentService.DeletePaymentMethod(paymentId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<IEnumerable<string>> GetNewMethods()
        {
            var newMethods = await paymentService.GetNewProviders();
            return ResponseToJsonHelper.GetJson<List<string>>(newMethods);
        }

        public async Task<IEnumerable<PaymentMethod>> GetPaymentMethods()
        {
            var methods = await paymentService.GetPaymentMethods();
            return ResponseToJsonHelper.GetJson<List<PaymentMethod>>(methods);
        }

        public async Task<string> UpdatePaymentMethod(EditPaymentDto data)
        {
            var response = await paymentService.UpdatePaymentMethod(data);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
        }
    }
}
