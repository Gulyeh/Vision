using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionClient.Core.Dtos;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class AddCurrencyControlViewModel : BindableBase
    {
        private AddCurrencyDto currencyModel = new();
        public AddCurrencyDto CurrencyModel
        {
            get { return currencyModel; }
            set { SetProperty(ref currencyModel, value); }
        }

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

        private readonly ICurrencyRepository currencyRepository;
        public DelegateCommand ExecuteCommand { get; }

        public AddCurrencyControlViewModel(ICurrencyRepository currencyRepository)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            this.currencyRepository = currencyRepository;
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!CurrencyModel.Validator())
            {
                ErrorText = "Please fill all data";
                return;
            }
            IsButtonEnabled = false;

            try
            {
                ErrorText = await currencyRepository.AddPackage(CurrencyModel);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }
    }
}
