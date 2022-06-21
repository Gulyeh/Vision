using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Models;

namespace VisionClient.Core.Dtos
{
    public class GetPagedNewsDto
    {
        public GetPagedNewsDto()
        {
            NewsList = new();
        }

        public List<NewsModel> NewsList { get; set; }
        public int MaxPages { get; set; }
    }
}
