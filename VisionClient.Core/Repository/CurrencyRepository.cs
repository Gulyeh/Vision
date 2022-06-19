using VisionClient.Core.Dtos;
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

        public async Task<string> AddPackage(AddCurrencyDto data)
        {
            var response = await currencyService.AddCurrencyPackage(data);
            if (response is null) throw new Exception();

            return ResponseToJsonHelper.GetJson(response);
        }

        public async Task<IEnumerable<CoinPackageModel>> GetPackages()
        {
            var packages = await currencyService.GetCurrencies();
            return ResponseToJsonHelper.GetJson<List<CoinPackageModel>>(packages);
        }
    }
}
