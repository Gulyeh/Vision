using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VisionClient.Scrolling
{
    internal static class HandleScrollingToParent
    {
        public static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
                eventArg.RoutedEvent = UIElement.MouseWheelEvent;
                eventArg.Source = sender;
                var parent = ((Control)sender).Parent as UIElement;
                if (parent != null) parent.RaiseEvent(eventArg);
            }
        }
    }
}
