using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface IPaymentService
    {
        Task<ResponseDto> GetPaymentMethods();
    }
}
