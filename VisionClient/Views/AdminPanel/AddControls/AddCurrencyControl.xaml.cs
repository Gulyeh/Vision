using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.AdminPanel.AddControls
{
    /// <summary>
    /// Logika interakcji dla klasy AddCurrencyControl.xaml
    /// </summary>
    public partial class AddCurrencyControl : UserControl
    {
        public AddCurrencyControl()
        {
            InitializeComponent();
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
