using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class TFAControlViewModel : DialogHelper
    {
        public TFAControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void Execute(object? data)
        {
           
        }
    }
}
