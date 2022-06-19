using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesDataService_API.Messages
{
    public class PhotoData
    {
        public PhotoData(string photoUrl, string photoId, Guid gameId)
        {
            PhotoUrl = photoUrl;
            PhotoId = photoId;
            GameId = gameId;
        }

        public Guid GameId { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoId { get; set; }
    }
}