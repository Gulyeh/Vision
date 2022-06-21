using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesDataService_API.Messages
{
    public class GameProductData
    {
        public GameProductData(string photoUrl, string photoId, Guid gameId, string name)
        {
            PhotoUrl = photoUrl;
            PhotoId = photoId;
            GameId = gameId;
            Name = name;
        }

        public Guid GameId { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoId { get; set; }
        public string Name { get; set; }
    }
}