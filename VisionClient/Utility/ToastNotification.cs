using Notification.Wpf;
using System.Windows.Media;
using VisionClient.Core;

namespace VisionClient.Utility
{
    public interface IToastNotification
    {
        void Show(string title, string message);
    }

    public class ToastNotification : IToastNotification
    {
        private readonly IStaticData staticData;
        private readonly NotificationContent content = new();

        public ToastNotification(IStaticData staticData)
        {
            content = new NotificationContent
            {
                Type = NotificationType.Information,
                RowsCount = 3,
                Background = (SolidColorBrush?)new BrushConverter().ConvertFrom("#0f3452"),
                Foreground = new SolidColorBrush(Colors.White)
            };

            this.staticData = staticData;
        }

        public void Show(string title, string message)
        {
            content.Title = title;
            content.Message = message;
            var areaName = staticData.IsMainWindowVisible ? "MainWindowArea" : string.Empty;

            NotificationManager notificationManager = new();
            notificationManager.Show(content, areaName);
        }
    }
}
