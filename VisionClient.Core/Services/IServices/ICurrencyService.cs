using VisionClient.Core.Dtos;

namespace VisionClient.Core.Services.IServices
{
    public interface ICurrencyService
    {
        Task<ResponseDto> GetCurrencies();
        Task<ResponseDto?> AddCurrencyPackage(AddCurrencyDto data);
        Task<ResponseDto?> DeletePackage(Guid packageId);
        Task<ResponseDto?> EditPackage(EditCurrencyDto data);
    }
}
