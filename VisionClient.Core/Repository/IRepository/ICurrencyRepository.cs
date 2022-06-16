using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<IEnumerable<CoinPackageModel>> GetPackages();
    }
}
