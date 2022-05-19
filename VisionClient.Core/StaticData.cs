using VisionClient.Core.Enums;
using VisionClient.Core.Models;

namespace VisionClient.Core
{
    public static class StaticData
    {
        public static string Access_Token = string.Empty;
        public static UserModel UserData = new UserModel()
        {
            Id = 12312313,
            PhotoUrl = "https://iso.500px.com/wp-content/uploads/2016/03/stock-photo-142984111.jpg",
            Description = "Some Test Description",
            UserName = "Gulyee",
            Status = Status.Away,
            EmailAddress = "test@email.com"
        };
    }
}
