using VisionClient.Core.Enums;
using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class BaseUserModel : NotifyPropertyChanged
    {
        public Guid UserId { get; set; }

        private string username = string.Empty;
        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        private string photoUrl = string.Empty;
        public string PhotoUrl
        {
            get => photoUrl;
            set
            {
                photoUrl = value;
                OnPropertyChanged();
            }
        }

        private string description = string.Empty;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged();
            }
        }

        private Status status;
        public Status Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged();
            }
        }    
    }
}
