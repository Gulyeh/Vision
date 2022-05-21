using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VisionClient.Core.Events;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class TFAControlViewModel : DialogHelper
    {
        private string authCode = string.Empty;
        public string AuthCode
        {
            get { return authCode; }
            set { SetProperty(ref authCode, value); }
        }

        private readonly IEventAggregator eventAggregator;

        public TFAControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public override void OnDialogClosed()
        {
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            Content = string.Empty;
            Content = parameters.GetValue<string>("content");
        }

        public override void Execute(object? data)
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish($"authCode:{AuthCode}");
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }
    }
}
