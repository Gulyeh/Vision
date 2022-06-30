using Syncfusion.Windows.Tools.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Extensions;

namespace VisionClient.Views.AdminPanel.AddControls
{
    /// <summary>
    /// Logika interakcji dla klasy AddCouponControl.xaml
    /// </summary>
    public partial class AddCouponControl : UserControl
    {
        public AddCouponControl()
        {
            InitializeComponent();
        }

        private void ChangedCouponType(object sender, SelectionChangedEventArgs e)
        {
            Game.SelectedIndex = -1;
            Package.SelectedIndex = -1;

            Amount.Visibility = Visibility.Collapsed;
            DiscountType.Visibility = Visibility.Collapsed;
            DiscountAmount.Visibility = Visibility.Collapsed;
            Game.Visibility = Visibility.Collapsed;
            Package.Visibility = Visibility.Collapsed;

            if (sender is not ComboBoxAdv combo) return;
            switch (combo.SelectedIndex)
            {
                case 1:
                    Amount.Visibility = Visibility.Visible;
                    break;
                case 2:
                    DiscountType.Visibility = Visibility.Visible;
                    DiscountAmount.Visibility = Visibility.Visible;
                    break;
                case 3:
                    Game.Visibility = Visibility.Visible;
                    break;
                case 4:
                    Game.Visibility = Visibility.Visible;
                    Package.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
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
