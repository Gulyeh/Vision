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
using VisionClient.Utility;
using VisionClient.Views;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class LogoutControlViewModel : DialogHelper
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IXMLCredentials XMLCredentials;

        public LogoutControlViewModel(IEventAggregator eventAggregator, IXMLCredentials XMLCredentials) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.XMLCredentials = XMLCredentials;
        }

        public override void Execute(object? window)
        {
            XMLCredentials.SaveCredentials("", "", false);
            var bs = new LoginBootstrapper();
            bs.Run();
            CloseParentWindowHelper.Close(window as UserControl);
        }
    }
}
