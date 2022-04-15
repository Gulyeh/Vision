using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersService_API.Messages;

namespace UsersService_API.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<bool> ChangeFunds(CurrencyDto data);
    }
}