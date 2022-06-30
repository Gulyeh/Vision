using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class UserDataModel : BaseUserModel
    {
        public UserDataModel()
        {
            Email = string.Empty;
            access_token = string.Empty;
            Role = string.Empty;
        }

        private int currencyValue;
        public int CurrencyValue
        {
            get => currencyValue;
            set
            {
                currencyValue = value;
                OnPropertyChanged();
            }
        }
        public string Email { get; set; }
        private string access_token;
        public string Access_Token
        {
            get => access_token;
            set
            {
                access_token = value;
                Role = GetJWTRoleHelper.GetRole(value);
            }
        }
        public string Role { get; private set; }
    }
}
