using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ChangePhotoControlViewModel : DialogHelper
    {
        public ChangePhotoControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void Execute(object? data)
        {
     
        }
    }
}
