using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Messages
{
    public class DeleteGameDto
    {
        public DeleteGameDto()
        {
            ProductsId = new List<Guid>();
        }

        public Guid GameId { get; set; }
        public IEnumerable<Guid> ProductsId { get; set; }
    }
}