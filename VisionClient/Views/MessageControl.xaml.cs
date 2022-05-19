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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionClient.Core.Models;
using VisionClient.Scrolling;
using VisionClient.ViewModels;

namespace VisionClient.Views
{
    public partial class MessageControl : UserControl
    {
        public MessageControl()
        {
            InitializeComponent();
        }

        private void Message_KeyPressed(object sender, KeyEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if (textBox is null) return;

            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) textBox.AppendText("\n");
            }
        }
        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }
        private void Messages_FocusLastMessage(object sender, SizeChangedEventArgs e)
        {
            Decorator? listviewDecorator = VisualTreeHelper.GetChild(MessagesListView, 0) as Decorator;
            if (listviewDecorator is null) return;

            ScrollViewer? scrollViewer = listviewDecorator.Child as ScrollViewer;
            if(scrollViewer is null) return;

            scrollViewer.ScrollToEnd();
        }

        private void MessageBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBox? textBox = sender as TextBox;
            if(textBox is null) return;

            var grid = MessageGrid.RowDefinitions[2];
            var att = AttachmentList.Items.Count > 0 ? 100 : 0;
            var value = textBox.ActualHeight + 45 + att;

            if (textBox.LineCount > 1) grid.Height = new GridLength(value, GridUnitType.Pixel);
            else grid.Height = new GridLength(70, GridUnitType.Pixel);
        }

        private void AttachmentList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView? listView = sender as ListView;
            if (listView is null) return;
            var grid = MessageGrid.RowDefinitions[2];

            if (listView.Items.Count == 1) grid.Height = new GridLength(grid.ActualHeight + 30, GridUnitType.Pixel);
            else if(listView.Items.Count == 0) grid.Height = new GridLength(grid.ActualHeight - 30, GridUnitType.Pixel);
        }

        private void Deselect_Item(object sender, MouseButtonEventArgs e)
        {
            ListView? listView = sender as ListView;
            if (listView is null) return;
            listView.SelectedItem = null;
        }
    }
}
