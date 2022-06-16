using Prism.Events;
using System;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class DeleteAccountControlViewModel : DialogHelper
    {
        private string password = String.Empty;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        public DeleteAccountControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        protected override void Execute(object? data)
        {

        }
    }
}
