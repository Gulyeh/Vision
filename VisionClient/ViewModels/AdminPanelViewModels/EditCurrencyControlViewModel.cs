using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class EditCurrencyControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private bool isButtonEnabled = true;
        public bool IsButtonEnabled
        {
            get { return isButtonEnabled; }
            set { SetProperty(ref isButtonEnabled, value); }
        }

        private EditCurrencyDto currencyModel = new();
        public EditCurrencyDto CurrencyModel
        {
            get { return currencyModel; }
            set { SetProperty(ref currencyModel, value); }
        }

        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly ICurrencyRepository currencyRepository;

        public DelegateCommand BackwardCommand { get; }
        public DelegateCommand ExecuteCommand { get; }

        public EditCurrencyControlViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, ICurrencyRepository currencyRepository)
        {
            BackwardCommand = new DelegateCommand(NavigateToCurrency);
            ExecuteCommand = new DelegateCommand(Execute);

            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.currencyRepository = currencyRepository;
            eventAggregator.GetEvent<SendEvent<CoinPackageModel>>().Subscribe(x =>
            {
                var newCurrency = new EditCurrencyDto()
                {
                    Title = x.Title,
                    Price = x.Price,
                    Discount = x.Discount,
                    Id = x.Id,
                    IsAvailable = x.IsAvailable,
                    Details = x.Details,
                    Amount = x.Amount
                };
                CurrencyModel = newCurrency;
            });
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if(!CurrencyModel.Validator())
            {
                ErrorText = "Fill all data";
                return;
            }
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await currencyRepository.EditPackage(CurrencyModel);
                if (success)
                {
                    eventAggregator.GetEvent<SendEvent<string>>().Publish("UpdateCoins");
                    NavigateToCurrency();
                }
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private void NavigateToCurrency() => regionManager.RequestNavigate("AdminPanelRegion", "ManageCurrencyControl");
    }
}
