using Identity_API.Dtos;

namespace Identity_API.Repository.IRepository
{
    public interface IAccessRepository
    {
        Task<ResponseDto> BanUser(BannedUsersDto data, Guid adminId);
        Task<ResponseDto> UnbanUser(Guid userId);
        Task<ResponseDto> GetServerData(Guid sessionToken, Guid userId);
        Task<IEnumerable<string>> GetRoles();
        Task<ResponseDto> ChangeUserRole(Guid userId, string role, Guid requesterId);
    }
}