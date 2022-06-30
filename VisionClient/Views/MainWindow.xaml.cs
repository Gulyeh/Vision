using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using VisionClient.Helpers;

namespace VisionClient.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SourceInitialized += Window_SourceInitialized;
        }

        void Window_SourceInitialized(object? sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(handle)?.AddHook(WindowProc);
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0024:
                    FullScreenHelper.WmGetMinMaxInfo(hwnd, lParam, (int)MinWidth, (int)MinHeight);
                    handled = true;
                    break;
            }

            return (IntPtr)0;
        }
        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (e.ClickCount == 1) DragMove();
            else if (e.ClickCount == 2)
            {
                MaxHeight = SystemParameters.VirtualScreenHeight - 10;
                MaxWidth = SystemParameters.VirtualScreenWidth - 10;
                WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
            }
        }
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }
        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            DisposeNotify();
            Environment.Exit(0);
        }

        private void MaximizeApplication(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal) WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized) WindowState = WindowState.Normal;
        }
        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_Closed(object sender, EventArgs e) => DisposeNotify();

        private void DisposeNotify() => NotfyIcon.Dispose();
    }
}
