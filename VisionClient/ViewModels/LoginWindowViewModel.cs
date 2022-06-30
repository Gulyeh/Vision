using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Core.Helpers.Aggregators;

namespace VisionClient.ViewModels
{
    internal class LoginWindowViewModel : BindableBase
    {
        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private Visibility shadowVisibility = Visibility.Collapsed;
        public Visibility ShadowVisibility
        {
            get { return shadowVisibility; }
            set { SetProperty(ref shadowVisibility, value); }
        }

        public LoginWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            regionManager.RegisterViewWithRegion("LoginContent", "LoginControl");

            eventAggregator.GetEvent<SendEvent<LoginWindowLoading>>().Subscribe(x =>
            {
                LoadingVisibility = x.LoadingVisibility == false ? Visibility.Collapsed : Visibility.Visible;
            });

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                ShadowVisibility = ShadowVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }, ThreadOption.PublisherThread, false, x => x == "ShadowLoginWindow");
        }
    }
}
