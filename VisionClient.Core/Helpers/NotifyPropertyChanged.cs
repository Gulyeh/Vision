using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VisionClient.Core.Helpers
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged is not null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
