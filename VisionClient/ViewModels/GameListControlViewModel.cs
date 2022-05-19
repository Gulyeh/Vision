using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class GameListControlViewModel : BindableBase
    {
        private readonly IEventAggregator eventAggregator;
        public ObservableCollection<GameModel> gameList { get; set; }

        private GameModel? selectedGame;
        public GameModel? SelectedGame
        {
            get { return selectedGame; }
            set 
            { 
                SetProperty(ref selectedGame, value);
                if(value is not null) GameSelected(value);
            }
        }

        public GameListControlViewModel(IEventAggregator eventAggregator)
        {
            gameList = new ObservableCollection<GameModel>();
            this.eventAggregator = eventAggregator;

            var game = new GameModel()
            {
                IsPurchased = true,
                Title = "Swords of Legends Online",
                IconUrl = "https://external-preview.redd.it/1xGR2LYs_5ILXWtvqWsbE4I1G_OAJ3bHccvn_-XIPWs.jpg?auto=webp&s=030a9f1c6f7a7a2128e8bff83eab7a600ab6eada",
                PhotoUrl = "https://mmos.com/wp-content/uploads/2021/04/swords-of-legends-online-character-summoner-banner.jpg",
                News = new List<NewsModel>()
                {
                    new NewsModel()
                    {
                        Date = System.DateTime.Now,
                        Title = "Check out our newest game",
                        PhotoUrl = "https://www.allkeyshop.com/blog/wp-content/uploads/solo-banner.jpg",
                        Content = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aenean rhoncus, sem in malesuada vehicula, est erat tristique lacus, rhoncus consequat sem tellus at nibh. Phasellus a tristique risus. Nulla ac metus et massa congue scelerisque. In non metus eu felis laoreet dictum et nec dui. Duis mi mi, condimentum eget molestie eu, commodo a ipsum. Phasellus faucibus, dui a dictum aliquet, lacus metus consequat quam, non tristique nisi arcu lobortis erat. Fusce tellus massa, egestas varius molestie non, consectetur non nisi. Donec eleifend blandit massa at mollis. Etiam velit metus, sagittis at ex placerat, dignissim porta leo. Nunc ultrices, est vulputate maximus venenatis, tortor justo rhoncus risus, a mattis velit augue non dolor. Sed ullamcorper ornare posuere. Nullam imperdiet malesuada tincidunt.Suspendisse potenti. Donec consectetur massa ac leo venenatis pellentesque. Vestibulum sollicitudin erat urna, a venenatis nunc lacinia vitae. Vivamus condimentum non metus ut consectetur. Sed lacinia nisl arcu, at lobortis metus semper elementum. Aliquam egestas vel ipsum id pellentesque. Ut scelerisque ante mauris, id imperdiet risus feugiat sed. Cras leo nunc, eleifend sit amet felis sed, viverra condimentum nisi. Aenean viverra bibendum tellus, sed lacinia justo varius vitae. Quisque velit mauris, hendrerit eu vehicula sed, hendrerit nec est. Nam cursus gravida est, sed fringilla nunc fermentum at. Phasellus vitae lobortis leo, vel gravida urna. Nunc in accumsan ante, nec convallis elit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Donec tristique ipsum at placerat congue."
                    }
                },
                Products = new List<ProductsModel>()
                {
                    new ProductsModel()
                    {
                        Discount = 50,
                        PhotoUrl = "https://cdn.cloudflare.steamstatic.com/steamcommunity/public/images/items/1418100/1331a013c2d9fd9d6418bd80ddf1d6e70a619752.jpg",
                        Title = "Reaper Pack",
                        Price = 99.99M,
                        IsAvailable = true
                    }
                }
            };

            foreach (var product in game.Products) product.Discounted();
            gameList.Add(game);
        }

        private void GameSelected(GameModel game)
        {
            eventAggregator.GetEvent<SendEvent<GameModel>>().Publish(game);
        }
    }
}
