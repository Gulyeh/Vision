using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.SettingsControls
{
    /// <summary>
    /// Logika interakcji dla klasy AuthenticationControl.xaml
    /// </summary>
    public partial class AuthenticationControl : UserControl
    {
        public AuthenticationControl()
        {
            InitializeComponent();
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
