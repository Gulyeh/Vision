using Newtonsoft.Json;
using Prism.Events;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Events;
using VisionClient.Core.Models.Account;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class UserBannedControlViewModel : DialogHelper
    {
        private BanModel userbanned = new();
        public BanModel UserBanned
        {
            get { return userbanned; }
            set {  SetProperty(ref userbanned, value); }
        }

        public UserBannedControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {

        }

        public override void OnDialogClosed()
        {
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            var stringData = parameters.GetValue<string>("content");
            var json = JsonConvert.DeserializeObject<BanModel>(stringData);
            if(json is not null) UserBanned = json;
        }

        public override void Execute(object? data) 
        {
        }
    }
}
