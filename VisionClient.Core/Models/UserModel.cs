namespace VisionClient.Core.Models
{
    public class UserModel : BaseUserModel
    {
        private bool hasUnreadMessages;
        public bool HasUnreadMessages
        {
            get { return hasUnreadMessages; }
            set
            {
                hasUnreadMessages = value;
                OnPropertyChanged();
            }
        }

        private bool isBlocked;
        public bool IsBlocked
        {
            get => isBlocked;
            set
            {
                isBlocked = value;
                OnPropertyChanged();
            }
        }
    }
}
