using VisionClient.Core.Models;

namespace VisionClient.Core.Helpers.Aggregators
{
    public class HomeToDetails
    {
        public HomeToDetails(GameModel game)
        {
            Game = game;
        }

        public GameModel Game { get; private set; }
    }
}
