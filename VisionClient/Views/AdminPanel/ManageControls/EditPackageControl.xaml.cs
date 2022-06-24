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
    /// Logika interakcji dla klasy AddGamePackageControl.xaml
    /// </summary>
    public partial class EditPackageControl : UserControl
    {
        public EditPackageControl()
        {
            InitializeComponent();
        }

        private void CheckNumeric(object sender, TextCompositionEventArgs e) => e.Handled = e.Text.IsNumeric();
    }
}
