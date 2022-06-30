using Prism.Events;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers.Aggregators;

namespace VisionClient.Helpers
{
    internal class LoginWindowLoadingVisibilityHelper
    {
        private readonly IEventAggregator eventAggregator;

        public LoginWindowLoadingVisibilityHelper(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void IsLoadingVisible(bool data)
        {
            eventAggregator.GetEvent<SendEvent<LoginWindowLoading>>().Publish(new LoginWindowLoading(data));
        }
    }
}
