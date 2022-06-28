using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionClient.Core.Dtos
{
    public class GiveProductDto
    {
        public Guid GameId { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        public bool Validation() => GameId != Guid.Empty && UserId != Guid.Empty;
    }
}
