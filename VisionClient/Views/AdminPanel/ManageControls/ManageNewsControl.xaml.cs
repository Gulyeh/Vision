using Syncfusion.UI.Xaml.Grid;
using System.Windows.Controls;
using System.Windows.Input;
using VisionClient.Scrolling;

namespace VisionClient.Views.AdminPanel.ManageControls
{
    /// <summary>
    /// Logika interakcji dla klasy ManageNewsControl.xaml
    /// </summary>
    public partial class ManageNewsControl : UserControl
    {
        double autoHeight;
        GridRowSizingOptions gridRowResizingOptions = new();

        public ManageNewsControl()
        {
            InitializeComponent();
        }
        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HandleScrollingToParent.HandlePreviewMouseWheel(sender, e);
        }
        private void dataGrid_QueryRowHeight(object? sender, QueryRowHeightEventArgs e)
        {
            SfDataGrid? dataGrid = sender as SfDataGrid;
            if (dataGrid is null) return;

            if (dataGrid.GridColumnSizer.GetAutoRowHeight(e.RowIndex, gridRowResizingOptions, out autoHeight))
            {
                if (autoHeight > 24)
                {
                    e.Height = autoHeight + 10;
                    e.Handled = true;
                }
            }
        }
    }
}
