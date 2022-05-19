using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisionClient.Core.Events;
using VisionClient.Helpers;
using VisionClient.Views;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class LogoutControlViewModel : DialogHelper
    {
        public LogoutControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public override void Execute(object? window)
        {
            var bs = new LoginBootstrapper();
            bs.Run();
            CloseParentWindowHelper.Close(window as UserControl);
        }
    }
}
