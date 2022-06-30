using Prism.Events;
using Prism.Mvvm;
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

        public IStaticData StaticData { get; }

        private readonly IEventAggregator eventAggregator;

        public GameListControlViewModel(IEventAggregator eventAggregator, IStaticData StaticData)
        {
            this.eventAggregator = eventAggregator;
            this.StaticData = StaticData;
            if (StaticData.GameModels.Count > 0) SelectedGame = StaticData.GameModels[0];
        }

        private void GameSelected(GameModel game) => eventAggregator.GetEvent<SendEvent<GameModel>>().Publish(game);
    }
}
