using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ApplyCouponControlViewModel : DialogHelper
    {
        private string couponCode = string.Empty;
        public string CouponCode
        {
            get { return couponCode; }
            set {  SetProperty(ref couponCode, value); }
        }

        public ApplyCouponControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        public override void Execute(object? data)
        {
           
        }
    }
}
