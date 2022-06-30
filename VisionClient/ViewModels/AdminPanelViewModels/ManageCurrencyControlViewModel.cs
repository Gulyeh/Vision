using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageCurrencyControlViewModel : BindableBase
    {
        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }

        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }


        private readonly ICurrencyRepository currencyRepository;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public DelegateCommand<CoinPackageModel> EditCurrencyCommand { get; }
        public DelegateCommand<Guid?> DeleteCurrencyCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public ObservableCollection<CoinPackageModel> CoinsList { get; }

        public ManageCurrencyControlViewModel(ICurrencyRepository currencyRepository, IEventAggregator eventAggregator, IRegionManager regionManager,
            IDialogService dialogService)
        {
            EditCurrencyCommand = new DelegateCommand<CoinPackageModel>(EditCurrency);
            DeleteCurrencyCommand = new DelegateCommand<Guid?>(DeleteCurrency);
            ExecuteCommand = new DelegateCommand(GetPackages);

            CoinsList = new();

            this.currencyRepository = currencyRepository;
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;

            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                GetPackages();
            }, ThreadOption.PublisherThread, false, x => x.Equals("UpdateCoins"));
        }


        private async void GetPackages()
        {
            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                var list = await currencyRepository.GetPackages();
                CoinsList.Clear();
                foreach (var package in list)
                {
                    if (package.OldPrice is not null) package.Price = (decimal)package.OldPrice;
                    CoinsList.Add(package);
                }
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async void DeleteCurrency(Guid? currencyId)
        {
            if (currencyId is null || currencyId == Guid.Empty) return;

            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
                {
                    { "title", "Confirm Delete" },
                    { "message", "Are you sure, you want to delete this package?" }
                }, x =>
                {
                    result = x.Result switch
                    {
                        ButtonResult.OK => true,
                        ButtonResult.Cancel => false,
                        _ => false,
                    };
                });
            if (!result) return;

            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                (bool success, ErrorText) = await currencyRepository.DeletePackage((Guid)currencyId);
                if (success) CoinsList.Remove(CoinsList.First(x => x.Id == currencyId));

                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private void EditCurrency(CoinPackageModel model)
        {
            regionManager.RequestNavigate("AdminPanelRegion", "EditCurrencyControl");
            eventAggregator.GetEvent<SendEvent<CoinPackageModel>>().Publish(model);
        }
    }
}
