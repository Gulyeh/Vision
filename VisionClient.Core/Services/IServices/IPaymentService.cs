using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IPaymentService
    {
        Task<ResponseDto> GetPaymentMethods();
        Task<ResponseDto> GetNewProviders();
        Task<ResponseDto?> CreatePaymentMethod(AddPaymentMethodDto data);
        Task<ResponseDto?> DeletePaymentMethod(Guid paymentMethodId);
        Task<ResponseDto?> UpdatePaymentMethod(EditPaymentDto data);
        Task<ResponseDto?> GetUserPayments();
    }
}
