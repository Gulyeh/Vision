using Prism.Events;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.DialogsViewModels
{
    internal class ConfirmControlViewModel : DialogHelper
    {
        public ConfirmControlViewModel(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        protected override void Execute(object? data)
        {

        }
    }
}
