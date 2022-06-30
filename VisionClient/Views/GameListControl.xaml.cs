using System.Windows;
using System.Windows.Controls;
using VisionClient.Core.Enums;
using VisionClient.Scrolling;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LibraryControl.xaml
    /// </summary>
    public partial class GameListControl : UserControl
    {
        public GameListControl()
        {
            InitializeComponent();
        }

        private void GameList_OnRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void GameListView_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            LeftScrollButton.Visibility = e.HorizontalOffset == 0 ? Visibility.Hidden : Visibility.Visible;
            RightScrollButton.Visibility = e.HorizontalOffset + e.ViewportWidth == e.ExtentWidth ? Visibility.Hidden : Visibility.Visible;
        }

        private async void RightScrollButton_Click(object sender, RoutedEventArgs e)
        {
            await SmoothScrolling.SmoothScroll(ScrollType.Right, "GameListView", this, 600, 40);
        }

        private async void LeftScrollButton_Click(object sender, RoutedEventArgs e)
        {
            await SmoothScrolling.SmoothScroll(ScrollType.Left, "GameListView", this, 600, 40);
        }
    }
}
