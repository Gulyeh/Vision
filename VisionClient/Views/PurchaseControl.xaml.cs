using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Scrolling;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy PurchaseControl.xaml
    /// </summary>
    public partial class PurchaseControl : UserControl
    {
        public PurchaseControl()
        {
            InitializeComponent();
        }

        private void PaymentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentList.SelectedIndex > -1) PayButton.IsEnabled = true;
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }

        private void ResetControls_Loaded(object sender, RoutedEventArgs e)
        {
            PaymentList.SelectedIndex = -1;
            CodeText.Text = string.Empty;
            PayButton.IsEnabled = false;
        }
    }
}
