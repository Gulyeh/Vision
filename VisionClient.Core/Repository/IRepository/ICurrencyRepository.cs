using VisionClient.Core.Dtos;
using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<CoinPackageModel>> GetPackages();
        Task<(bool, string)> AddPackage(AddCurrencyDto data);
        Task<(bool, string)> DeletePackage(Guid packageId);
        Task<(bool, string)> EditPackage(EditCurrencyDto data);
    }
}
