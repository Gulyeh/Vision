using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Scrolling;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy HomeControl.xaml
    /// </summary>
    public partial class HomeControl : UserControl
    {
        public HomeControl()
        {
            InitializeComponent();
        }

        private void DeselectGameList_Item(object sender, RoutedEventArgs e)
        {
            GameList.SelectedItem = null;
        }
        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }

        private void PreventRightMouseButton(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
