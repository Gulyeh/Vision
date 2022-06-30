using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Scrolling;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy FriendsControl.xaml
    /// </summary>
    public partial class FriendsControl : UserControl
    {
        public FriendsControl()
        {
            InitializeComponent();
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }

        private void ListView_DisableRightClickSelection(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
