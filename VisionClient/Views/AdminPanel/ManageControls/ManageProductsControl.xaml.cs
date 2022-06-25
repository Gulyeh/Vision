using Syncfusion.UI.Xaml.Grid;
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
using VisionClient.Scrolling;

namespace VisionClient.Views.AdminPanel.ManageControls
{
    /// <summary>
    /// Logika interakcji dla klasy ManageNewsControl.xaml
    /// </summary>
    public partial class ManageProductsControl : UserControl
    {
        double autoHeight;
        GridRowSizingOptions gridRowResizingOptions = new();

        public ManageProductsControl()
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
