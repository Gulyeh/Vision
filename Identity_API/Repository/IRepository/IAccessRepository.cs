using Identity_API.Dtos;

namespace Identity_API.Repository.IRepository
{
    public interface IAccessRepository
    {
        Task<ResponseDto> BanUser(BannedUsersDto data);
        Task<ResponseDto> UnbanUser(Guid userId);
    }
}