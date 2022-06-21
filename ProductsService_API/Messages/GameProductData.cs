using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsService_API.Messages
{
    public class GameProductData
    {
        public GameProductData()
        {
            PhotoUrl = string.Empty;
            PhotoId = string.Empty;
            Name = string.Empty;
        }

        public Guid GameId { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoId { get; set; }
        public string Name { get; set; }
    }
}