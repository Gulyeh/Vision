using VisionClient.Core.Dtos;
using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<CoinPackageModel>> GetPackages();
        Task<(bool, string)> AddPackage(AddCurrencyDto data);
    }
}
