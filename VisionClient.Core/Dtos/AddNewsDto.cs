using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Dtos
{
    public class AddNewsDto
    {
        public AddNewsDto()
        {
            Title = string.Empty;
            Content = string.Empty;
            Photo = string.Empty;
        }

        public string Title { get; set; }
        public Guid GameId { get; set; }
        public string Content { get; set; }
        public string Photo { get; set; }

        public bool Validation() => !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Content);
    }
}
