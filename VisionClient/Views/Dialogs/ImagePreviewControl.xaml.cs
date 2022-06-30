using System.Windows;
using System.Windows.Controls;

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
