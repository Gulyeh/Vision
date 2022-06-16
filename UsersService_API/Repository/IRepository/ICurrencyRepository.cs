using UsersService_API.Messages;

namespace UsersService_API.Repository.IRepository
{
    public interface ICurrencyRepository
    {
        Task<bool> ChangeFunds(CurrencyDto data);
    }
}