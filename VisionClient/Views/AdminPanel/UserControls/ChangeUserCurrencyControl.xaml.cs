using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.AdminPanel.UserControls
{
    /// <summary>
    /// Logika interakcji dla klasy ToggleBanAccessControl.xaml
    /// </summary>
    public partial class ChangeUserCurrencyControl : UserControl
    {
        public ChangeUserCurrencyControl()
        {
            InitializeComponent();
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
