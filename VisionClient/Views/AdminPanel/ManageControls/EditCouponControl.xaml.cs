using Syncfusion.Windows.Tools.Controls;
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
