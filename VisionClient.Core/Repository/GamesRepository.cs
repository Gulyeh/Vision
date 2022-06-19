﻿using VisionClient.Core.Dtos;
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

        public async Task<string> AddGame(AddGameDto data)
        {
            var response = await gamesService.AddGame(data);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
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
            if (json is null) throw new Exception();
            return json;
        }

        public async Task<string> AddNews(AddNewsDto data)
        {
            var response = await gamesService.AddNews(data);
            if (response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson(response);
        }
    }
}