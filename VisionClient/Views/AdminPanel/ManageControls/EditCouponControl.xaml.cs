using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.AdminPanel.ManageControls
{
    /// <summary>
    /// Logika interakcji dla klasy AddCouponControl.xaml
    /// </summary>
    public partial class EditCouponControl : UserControl
    {
        public EditCouponControl()
        {
            InitializeComponent();
        }

        private void ChangedIsLimited(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ComboBoxAdv combo) return;
            if (combo.SelectedIndex == 1 && UsesAmount is not null) UsesAmount.Visibility = Visibility.Visible;
            else if (UsesAmount is not null) UsesAmount.Visibility = Visibility.Collapsed;
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
