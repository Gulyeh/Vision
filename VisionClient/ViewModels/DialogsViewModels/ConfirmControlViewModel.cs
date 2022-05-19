using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ConfirmControlViewModel : DialogHelper
    {
        public ConfirmControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void Execute(object? data)
        {
           
        }
    }
}
