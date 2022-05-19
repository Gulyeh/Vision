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
    /// Logika interakcji dla klasy ImagePreviewControl.xaml
    /// </summary>
    public partial class ImagePreviewControl : UserControl
    {
        public ImagePreviewControl()
        {
            InitializeComponent();
        }

        private void Image_Loaded(object sender, RoutedEventArgs e)
        {
            ImagePreview.Height = this.ActualHeight / 1.2;
        }
    }
}
