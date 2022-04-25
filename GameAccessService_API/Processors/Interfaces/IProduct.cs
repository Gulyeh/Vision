using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameAccessService_API.Processors.Interfaces
{
    public interface IProduct
    {
        void SetData(Guid userId, Guid gameId, Guid? productId = null);
        Task AddToCache();
        Task SaveData();
    }
}