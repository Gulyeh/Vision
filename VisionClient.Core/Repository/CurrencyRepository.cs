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

        public async Task<(bool,string)> AddPackage(AddCurrencyDto data)
        {
            var response = await currencyService.AddCurrencyPackage(data);
            if (response is null) throw new Exception();

            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> DeletePackage(Guid packageId)
        {
            var response = await currencyService.DeletePackage(packageId);
            if (response is null) throw new Exception();

            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> EditPackage(EditCurrencyDto data)
        {
            var response = await currencyService.EditPackage(data);
            if (response is null) throw new Exception();

            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<IEnumerable<CoinPackageModel>> GetPackages()
        {
            var packages = await currencyService.GetCurrencies();
            return ResponseToJsonHelper.GetJson<List<CoinPackageModel>>(packages);
        }
    }
}
