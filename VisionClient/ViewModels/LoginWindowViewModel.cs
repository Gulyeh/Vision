using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            set {  SetProperty(ref loadingVisibility, value); }
        }

        public LoginWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            regionManager.RegisterViewWithRegion("LoginContent", "LoginControl");
            eventAggregator.GetEvent<SendEvent<LoginWindowLoading>>().Subscribe(x =>
            {
                LoadingVisibility = x.LoadingVisibility == false ? Visibility.Collapsed : Visibility.Visible;
            });
        }
    }
}
