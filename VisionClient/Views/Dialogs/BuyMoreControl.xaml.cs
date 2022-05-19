using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
