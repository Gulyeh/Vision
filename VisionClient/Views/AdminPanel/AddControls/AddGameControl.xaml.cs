using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.AdminPanel.AddControls
{
    /// <summary>
    /// Logika interakcji dla klasy AddGameControl.xaml
    /// </summary>
    public partial class AddGameControl : UserControl
    {
        public AddGameControl()
        {
            InitializeComponent();
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
