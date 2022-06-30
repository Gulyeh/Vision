using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VisionClient.Core.Enums;

namespace VisionClient.Scrolling
{
    internal static class SmoothScrolling
    {
        private static ScrollViewer? FindScrollViewer<T>(T control, string elementName) where T : FrameworkElement
        {
            ListView? list = control.FindName(elementName) as ListView;
            Decorator? decorator = VisualTreeHelper.GetChild(list, 0) as Decorator;
            ScrollViewer? scrollViewer = decorator?.Child as ScrollViewer;
            return scrollViewer ?? null;
        }

        public async static Task SmoothScroll<T>(ScrollType type, string elementName, T control, int scrollAmount, int scrollBy) where T : FrameworkElement
        {
            var scrollViewer = FindScrollViewer<T>(control, elementName);
            if (scrollViewer == null) return;

            while (scrollAmount > 0)
            {
                if (type == ScrollType.Right) scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + scrollBy);
                else if (type == ScrollType.Left) scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - scrollBy);
                scrollAmount -= scrollBy;
                await Task.Delay(1);
            }

            if (type == ScrollType.Left && scrollViewer.HorizontalOffset - 200 < 0) scrollViewer.ScrollToLeftEnd();
            else if (type == ScrollType.Right && scrollViewer.HorizontalOffset + 200 > scrollViewer.ScrollableWidth) scrollViewer.ScrollToRightEnd();
        }
    }
}
