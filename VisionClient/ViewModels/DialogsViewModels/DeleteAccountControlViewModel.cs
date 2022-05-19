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
using VisionClient.Core.Events;
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

        public override void Execute(object? data)
        {
        
        }
    }
}
