using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class CurrencyRepository : ICurrencyRepository
    {
        private readonly ICurrencyService currencyService;

        public CurrencyRepository(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public async Task<IEnumerable<CoinPackageModel>> GetPackages()
        {
            var packages = await currencyService.GetCurrencies();
            return ResponseToJsonHelper.GetJson<List<CoinPackageModel>>(packages);
        }
    }
}
