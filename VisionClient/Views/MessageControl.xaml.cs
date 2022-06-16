using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Scrolling;
using VisionClient.ViewModels;

namespace VisionClient.Views
{
    public partial class MessageControl : UserControl
    {
        private readonly MessageControlViewModel? viewModel;
        private int PreviousAttachemntsAmount = 0;
        private bool AutoScroll = true;

        public MessageControl()
        {
            InitializeComponent();
            viewModel = DataContext as MessageControlViewModel;

            if (viewModel is not null)
            {
                ((INotifyCollectionChanged)viewModel.NewMessageAttachments).CollectionChanged += (s, e) =>
                {
                    var grid = MessageGrid.RowDefinitions[2];
                    if (PreviousAttachemntsAmount == 0 && AttachmentList.Items.Count > 0)
                    {
                        PreviousAttachemntsAmount++;
                        grid.Height = new GridLength(grid.ActualHeight + 100, GridUnitType.Pixel);
                    }
                    else if (PreviousAttachemntsAmount == 1 && AttachmentList.Items.Count == 0)
                    {
                        PreviousAttachemntsAmount--;
                        grid.Height = new GridLength(grid.ActualHeight - 100, GridUnitType.Pixel);
                    }
                };
            }
        }




        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }

        private async void Message_KeyPressed(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            if (e.Key == Key.Enter)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) textBox.AppendText("\n");
            }
        }

        private void MessageBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            var grid = MessageGrid.RowDefinitions[2];
            var att = AttachmentList.Items.Count > 0 ? 100 : 0;
            var value = textBox.ActualHeight + 45 + att;

            if (textBox.LineCount > 1) grid.Height = new GridLength(value, GridUnitType.Pixel);
            else grid.Height = new GridLength(70 + att, GridUnitType.Pixel);

        }

        private void Deselect_Item(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListView listView) return;
            listView.SelectedItem = null;
        }

        private void ListView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void MessageScroll_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.ExtentHeightChange == 0)
            {
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight) AutoScroll = true;
                else AutoScroll = false;
            }

            if (AutoScroll && e.ExtentHeightChange != 0) scrollViewer.ScrollToEnd();
        }

        private async void Scrollviewer_GetMoreMessages(object sender, MouseWheelEventArgs e)
        {
            if (viewModel is null) return;
            ScrollViewer scrollViewer = (ScrollViewer)sender;

            if (scrollViewer.VerticalOffset == 0 && scrollViewer.ScrollableHeight > 0) await viewModel.GetMoreMessages();
        }
    }
}
