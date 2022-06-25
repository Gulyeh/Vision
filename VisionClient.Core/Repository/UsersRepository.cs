using Newtonsoft.Json;
using VisionClient.Core.Helpers;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Core.Services.IServices;

namespace VisionClient.Core.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IUsersService usersService;
        private readonly IStaticData StaticData;

        public UsersRepository(IUsersService usersService, IStaticData staticData)
        {
            this.usersService = usersService;
            this.StaticData = staticData;
        }

        public async Task ChangePhoto(string image)
        {
            var response = await usersService.ChangePhoto(image);
            if (response == null || !response.isSuccess) throw new Exception();

            var responseData = response.Response.ToString();
            if (string.IsNullOrEmpty(responseData)) throw new Exception();

            StaticData.UserData.PhotoUrl = responseData;
        }

        public async Task<IEnumerable<BaseUserModel>> FindUsers(string contains)
        {
            var response = await usersService.FindUser(contains);
            if (response == null) return new List<BaseUserModel>();
            return ResponseToJsonHelper.GetJson<List<BaseUserModel>>(response);
        }

        public async Task<List<DetailedUserModel>> GetDetailedUsers(string containsString)
        {
            var response = await usersService.GetDetailedUsers(containsString);
            if(response is null) throw new Exception();
            return ResponseToJsonHelper.GetJson<List<DetailedUserModel>>(response);
        }
    }
}
