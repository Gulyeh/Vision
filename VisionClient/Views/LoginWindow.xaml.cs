using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void ExitApplication(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinimizeApplication(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.Tab:
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        private void Dockpanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1) DragMove();
        }
    }
}
