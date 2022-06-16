using Notification.Wpf;
using System.Windows.Media;

namespace VisionClient.Utility
{
    public interface IToastNotification
    {
        void Show(string title, string message);
    }

    public class ToastNotification : IToastNotification
    {
        private NotificationContent content = new();

        public ToastNotification()
        {
            content = new NotificationContent
            {
                Type = NotificationType.Information,
                RowsCount = 3,
                Background = (SolidColorBrush?)new BrushConverter().ConvertFrom("#0f3452"),
                Foreground = new SolidColorBrush(Colors.White)
            };
        }

        public void Show(string title, string message)
        {
            content.Title = title;
            content.Message = message;

            NotificationManager notificationManager = new();
            notificationManager.Show(content, "MainWindowArea");
        }
    }
}
