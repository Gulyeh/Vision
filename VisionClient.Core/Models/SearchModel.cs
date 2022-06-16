using VisionClient.Core.Helpers;

namespace VisionClient.Core.Models
{
    public class SearchModel : NotifyPropertyChanged
    {
        public SearchModel()
        {
            User = new();
        }

        public BaseUserModel User { get; set; }

        private bool isRequestable = true;
        public bool IsRequestable
        {
            get => isRequestable;
            set
            {
                isRequestable = value;
                OnPropertyChanged();
            }
        }
    }
}
