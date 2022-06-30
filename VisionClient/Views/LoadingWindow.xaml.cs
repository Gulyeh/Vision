using System.Windows;
using VisionClient.ViewModels;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow()
        {
            InitializeComponent();
            LoadingWindowViewModel? vm = this.DataContext as LoadingWindowViewModel;
            if (vm is not null) vm.TempWindow = this;
        }
    }
}
