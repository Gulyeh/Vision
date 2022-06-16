using Prism.Events;
using Prism.Services.Dialogs;
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
            eventAggregator.GetEvent<SendEvent<string>>().Publish("ShadowLoginWindow");
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish("ShadowLoginWindow");
            Content = string.Empty;
            Content = parameters.GetValue<string>("content");
        }

        protected override void Execute(object? data)
        {
            eventAggregator.GetEvent<SendEvent<string>>().Publish($"authCode:{AuthCode}");
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }
    }
}
