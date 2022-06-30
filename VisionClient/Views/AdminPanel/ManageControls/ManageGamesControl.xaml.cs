using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.AdminPanel.ManageControls
{
    /// <summary>
    /// Logika interakcji dla klasy AddGameControl.xaml
    /// </summary>
    public partial class ManageGamesControl : UserControl
    {
        public ManageGamesControl()
        {
            InitializeComponent();
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
