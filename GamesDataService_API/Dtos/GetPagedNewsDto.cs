using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GamesDataService_API.Dtos
{
    public class GetPagedNewsDto
    {
        public GetPagedNewsDto(IEnumerable<NewsDto> newsList)
        {
            NewsList = newsList;
            var pages = double.Parse((newsList.Count() / 10).ToString());
            MaxPages = (int)Math.Ceiling(pages);
        }

        public IEnumerable<NewsDto> NewsList { get; set; }
        public int MaxPages { get; init; }
    }
}