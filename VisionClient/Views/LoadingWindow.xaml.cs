using Prism.Events;
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
using VisionClient.Core.Events;
using VisionClient.ViewModels;

namespace VisionClient.Views
{
    /// <summary>
    /// Logika interakcji dla klasy LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                this.Close();
            }, ThreadOption.PublisherThread, false, x => x == "CloseLoading");
        }
    }
}
