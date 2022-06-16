using Prism.Events;
using Prism.Services.Dialogs;
using VisionClient.Core.Models;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class UserBannedControlViewModel : DialogHelper
    {
        private BanModel userbanned = new();
        public BanModel UserBanned
        {
            get { return userbanned; }
            set { SetProperty(ref userbanned, value); }
        }

        public UserBannedControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            UserBanned = parameters.GetValue<BanModel>("banModel");
        }

        protected override void Execute(object? data)
        {
        }
    }
}
