using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Core.Enums;
using VisionClient.Scrolling;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy GameDetailsControl.xaml
    /// </summary>
    public partial class GameDetailsControl : UserControl
    {
        public GameDetailsControl()
        {
            InitializeComponent();
        }

        private void ListView_OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void Deselect_ListItem(object sender, RoutedEventArgs e)
        {
            ListView? listView = sender as ListView;
            if (listView == null) return;
            listView.SelectedItem = null;
        }

        private void NewsListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LeftNewsScrollButton.Visibility = e.HorizontalOffset == 0 ? Visibility.Hidden : Visibility.Visible;
            RightNewsScrollButton.Visibility = e.HorizontalOffset + e.ViewportWidth == e.ExtentWidth ? Visibility.Hidden : Visibility.Visible;
        }

        private void PackageListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LeftPackageScrollButton.Visibility = e.HorizontalOffset == 0 ? Visibility.Hidden : Visibility.Visible;
            RightPackageScrollButton.Visibility = e.HorizontalOffset + e.ViewportWidth == e.ExtentWidth ? Visibility.Hidden : Visibility.Visible;
        }

        private async void LeftNewsScrollButton_Click(object sender, RoutedEventArgs e)
        {
            await SmoothScrolling.SmoothScroll(ScrollType.Left, "NewsListView", this, 800, 40);
        }

        private async void RightNewsScrollButton_Click(object sender, RoutedEventArgs e)
        {
            await SmoothScrolling.SmoothScroll(ScrollType.Right, "NewsListView", this, 800, 40);
        }

        private async void RightPackageScrollButton_Click(object sender, RoutedEventArgs e)
        {
            await SmoothScrolling.SmoothScroll(ScrollType.Right, "PackageListView", this, 600, 40);
        }

        private async void LeftPackageScrollButton_Click(object sender, RoutedEventArgs e)
        {
            await SmoothScrolling.SmoothScroll(ScrollType.Left, "PackageListView", this, 600, 40);
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }
    }
}
