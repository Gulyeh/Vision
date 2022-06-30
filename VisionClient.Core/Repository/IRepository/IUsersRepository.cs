using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<BaseUserModel>> FindUsers(string contains);
        Task ChangePhoto(string image);
        Task<List<DetailedUserModel>> GetDetailedUsers(string containsString);
        Task<string> ChangeCurrency(Guid userId, int Amount);
    }
}
