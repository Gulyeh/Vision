using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface ICurrencyService
    {
        Task<ResponseDto> GetCurrencies();
        Task<ResponseDto?> AddCurrencyPackage(AddCurrencyDto data);
    }
}
