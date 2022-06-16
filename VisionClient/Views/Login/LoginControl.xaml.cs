using System.Windows.Controls;
using VisionClient.ViewModels;

namespace VisionClient.Views.Login
{
    /// <summary>
    /// Logika interakcji dla klasy LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        readonly LoginControlViewModel? model;
        public LoginControl()
        {
            InitializeComponent();
            model = this.DataContext as LoginControlViewModel;
            if (model is not null) model.TempControl = this;
        }
    }
}
