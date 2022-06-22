using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProdcutsService_API.Messages
{
    public class DeleteGameDto
    {
        public DeleteGameDto(Guid gameId, IEnumerable<Guid> productsId)
        {
            GameId = gameId;
            ProductsId = productsId;
        }

        public Guid GameId { get; set; }
        public IEnumerable<Guid> ProductsId { get; set; }
    }
}