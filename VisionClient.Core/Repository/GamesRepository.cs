using VisionClient.Core.Dtos;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class GamesRepository : IGamesRepository
    {
        private readonly IGamesService gamesService;
        private readonly IStaticData StaticData;

        public GamesRepository(IGamesService gamesService, IStaticData staticData)
        {
            this.gamesService = gamesService;
            this.StaticData = staticData;
        }

        public async Task<(bool, string)> AddGame(AddGameDto data)
        {
            var response = await gamesService.AddGame(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task GetGames()
        {
            var games = await gamesService.GetGames();
            if (games is null) throw new Exception();

            var json = ResponseToJsonHelper.GetJson<List<GameModel>>(games);

            if (json is not null)
            {
                foreach (var game in json) StaticData.GameModels.Add(game);
            }
        }

        public async Task<IEnumerable<NewsModel>> GetNews(Guid gameId)
        {
            var news = await gamesService.GetNews(gameId);
            if (news is null) throw new Exception();

            return ResponseToJsonHelper.GetJson<List<NewsModel>>(news);
        }

        public async Task<GameProductModel> GetProducts(Guid gameId)
        {
            var products = await gamesService.GetProducts(gameId);
            if (products is null) throw new Exception();

            return ResponseToJsonHelper.GetJson<GameProductModel>(products);
        }

        public async Task<bool> OwnsProduct(Guid productId, Guid gameId)
        {
            var response = await gamesService.BoughtPackage(productId, gameId);
            if (response is null) throw new Exception();

            var json = ResponseToJsonHelper.GetJson<HasAccessDto>(response);
            if (json is null) return false;
            return json.HasAccess;
        }

        public async Task<BanModel?> CheckGameAccess(Guid gameId)
        {
            var response = await gamesService.CheckGameAccess(gameId);
            if (response is null) throw new Exception();
            if (response.isSuccess) return null;

            response.isSuccess = true;
            var json = ResponseToJsonHelper.GetJson<BanModel>(response);
            if (json is null || json.BanDate.Equals(default)) throw new Exception();
            return json;
        }

        public async Task<(bool, string)> AddNews(AddNewsDto data)
        {
            var response = await gamesService.AddNews(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> AddGamePackage(AddPackageDto data)
        {
            var response = await gamesService.AddGamePackage(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<string> EditGame(EditGameDto data)
        {
            var response = await gamesService.EditGame(data);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
        }

        public async Task<List<NewsModel>> GetPagedNews(Guid gameId)
        {
            var response = await gamesService.GetNews(gameId);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson<List<NewsModel>>(response);
        }

        public async Task<(bool, string)> DeleteNews(Guid gameId, Guid newsId)
        {
            var response = await gamesService.DeleteNews(gameId, newsId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> EditNews(EditNewsDto data)
        {
            var response = await gamesService.EditNews(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> DeleteGame(Guid gameId)
        {
            var response = await gamesService.DeleteGame(gameId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> DeleteProduct(Guid productId, Guid gameId)
        {
            var response = await gamesService.DeletePackage(productId, gameId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> EditPackage(EditPackageDto data)
        {
            var response = await gamesService.EditPackage(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> BanUser(BanGameDto data)
        {
            var response = await gamesService.BanUser(data);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> UnbanUser(Guid userId, Guid gameId)
        {
            var response = await gamesService.UnbanUser(userId, gameId);
            if (response is null) throw new Exception();
            return (response.isSuccess, ResponseToJsonHelper.GetJson(response));
        }

        public async Task<(bool, string)> CheckIfUserIsBanned(Guid userId, Guid gameId)
        {
            var response = await gamesService.CheckIfUserIsBanned(userId, gameId);
            if (response is null) throw new Exception();
            if (!response.isSuccess) return (false, ResponseToJsonHelper.GetJson(response));
            return ((bool)response.Response, string.Empty);
        }

        public async Task<string> GiveUserProduct(GiveProductDto data)
        {
            var response = await gamesService.GiveUserProduct(data);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
        }
    }
}
