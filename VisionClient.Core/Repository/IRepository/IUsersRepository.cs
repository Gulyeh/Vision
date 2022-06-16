using VisionClient.Core.Models;

namespace VisionClient.Core.Repository.IRepository
{
    public interface IUsersRepository
    {
        Task<IEnumerable<BaseUserModel>> FindUsers(string contains);
        Task ChangePhoto(string image);
    }
}
