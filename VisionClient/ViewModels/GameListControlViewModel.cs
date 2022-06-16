using Prism.Events;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using VisionClient.Core;
using VisionClient.Core.Events;
using VisionClient.Core.Models;

namespace VisionClient.ViewModels
{
    internal class GameListControlViewModel : BindableBase
    {
        private GameModel? selectedGame;
        public GameModel? SelectedGame
        {
            get { return selectedGame; }
            set
            {
                SetProperty(ref selectedGame, value);
                if (value is not null) GameSelected(value);
            }
        }

        private readonly IEventAggregator eventAggregator;
        public ObservableCollection<GameModel> GameList { get; }

        public GameListControlViewModel(IEventAggregator eventAggregator, IStaticData StaticData)
        {
            GameList = StaticData.GameModels;
            this.eventAggregator = eventAggregator;
            SelectedGame = GameList[0];
        }

        private void GameSelected(GameModel game)
        {
            eventAggregator.GetEvent<SendEvent<GameModel>>().Publish(game);
        }
    }
}
