using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Events;
using VisionClient.Helpers;

namespace VisionClient.ViewModels
{
    internal class LoadingWindowViewModel : BindableBase
    {
        private int loadingValue;
        public int LoadingValue
        {
            get { return loadingValue; }
            set {  SetProperty(ref loadingValue, value); }
        }

        private readonly IEventAggregator eventAggregator;

        public LoadingWindowViewModel(IEventAggregator eventAggregator)
        {
            Initialization();
            this.eventAggregator = eventAggregator;
        }

        private async void Initialization()
        {
            await Task.Delay(1000);
            LoadingValue = 25;
            await Task.Delay(1000);
            LoadingValue = 50;
            await Task.Delay(1000);
            LoadingValue = 75;
            await Task.Delay(1000);
            LoadingValue = 100;
            await Task.Delay(1000);
            var bs = new Bootstrapper();
            bs.Run();
            eventAggregator.GetEvent<SendEvent<string>>().Publish("CloseLoading");
        }
    }
}
