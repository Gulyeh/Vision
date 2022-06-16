using System.Windows.Controls;

namespace VisionClient.Views.Dialogs
{
    /// <summary>
    /// Logika interakcji dla klasy BuyMoreControl.xaml
    /// </summary>
    public partial class BuyMoreControl : UserControl
    {
        public BuyMoreControl()
        {
            InitializeComponent();
        }

        private void CoinsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentList.SelectedIndex > -1 && CoinsList.SelectedIndex > -1) PayButton.IsEnabled = true;
            else PayButton.IsEnabled = false;
        }

        private void PaymentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PaymentList.SelectedIndex > -1 && CoinsList.SelectedIndex > -1) PayButton.IsEnabled = true;
            else PayButton.IsEnabled = false;
        }
    }
}
