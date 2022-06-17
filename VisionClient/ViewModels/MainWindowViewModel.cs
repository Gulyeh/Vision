using AutoUpdaterDotNET;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Windows;
using System.Windows.Threading;
using VisionClient.Core;
using VisionClient.Core.Enums;
using VisionClient.Core.Events;
using VisionClient.SignalR;
using VisionClient.Utility;
using VisionClient.Views;

namespace VisionClient.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private Visibility borderVisibility = Visibility.Hidden;
        public Visibility BorderVisibility
        {
            get { return borderVisibility; }
            set { SetProperty(ref borderVisibility, value); }
        }

        private bool isWindowVisible = true;
        public bool IsWindowVisible
        {
            get { return isWindowVisible; }
            set 
            { 
                SetProperty(ref isWindowVisible, value);
                StaticData.IsMainWindowVisible = value;
            }
        }

        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IMessageService_Hubs messageService_Hubs;

        public DelegateCommand<string> NavigateCommand { get; }
        public DelegateCommand BuyMoreCommand { get; }
        public DelegateCommand UseCodeCommand { get; }
        public DelegateCommand<string> SwitchVisibilityCommand { get; }
        public IStaticData StaticData { get; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService, IStaticData staticData,
            IMessageService_Hubs messageService_Hubs)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            StaticData = staticData;
            this.messageService_Hubs = messageService_Hubs;
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(HomeControl));
            NavigateCommand = new DelegateCommand<string>(Navigate);
            UseCodeCommand = new DelegateCommand(OpenCodeDialog);
            BuyMoreCommand = new DelegateCommand(BuyMoreDialog);
            SwitchVisibilityCommand = new DelegateCommand<string>(SwitchVisibility);

            CheckUpdate.CheckTimer();
            eventAggregator.GetEvent<SendEvent<Visibility>>().Subscribe(x => BorderVisibility = x);
        }

        private void SwitchVisibility(string state)
        {
            switch (state)
            {
                case "Show":
                    IsWindowVisible = true;
                    break;
                case "Hide":
                    IsWindowVisible = false;
                    break;
                default:
                    break;
            }
        }

        private void Navigate(string uri)
        {
            if (messageService_Hubs.MessageHubConnection is not null) messageService_Hubs.Disconnect();
            regionManager.RequestNavigate("ContentRegion", uri);
        }

        private void OpenCodeDialog()
        {
            dialogService.ShowDialog("ApplyCouponControl", new DialogParameters
            {
                { "CodeType", CodeTypes.Currency }
            }, null);
        }

        private void BuyMoreDialog() => dialogService.ShowDialog("BuyMoreControl");
    }
}