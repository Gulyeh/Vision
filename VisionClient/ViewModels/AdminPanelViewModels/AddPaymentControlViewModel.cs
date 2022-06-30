using Prism;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using VisionClient.Core.Dtos;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class AddPaymentControlViewModel : BindableBase, IActiveAware
    {
        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                SetProperty(ref _isActive, value, RaiseIsActiveChanged);
                if (value) GetProviders();
            }
        }

        private object iconImage = new();
        public object IconImage
        {
            get { return iconImage; }
            set { SetProperty(ref iconImage, value); }
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

        private AddPaymentMethodDto paymentMethod = new();
        public AddPaymentMethodDto PaymentMethod
        {
            get { return paymentMethod; }
            set { SetProperty(ref paymentMethod, value); }
        }

        public event EventHandler? IsActiveChanged;
        protected virtual void RaiseIsActiveChanged() => IsActiveChanged?.Invoke(this, EventArgs.Empty);
        private readonly IPaymentRepository paymentRepository;

        public DelegateCommand OpenImageCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand ExecuteUpdateCommand { get; }
        public ObservableCollection<string> ProvidersList { get; set; }

        public AddPaymentControlViewModel(IPaymentRepository paymentRepository)
        {
            ProvidersList = new();
            ExecuteUpdateCommand = new DelegateCommand(GetProviders);
            ExecuteCommand = new DelegateCommand(Execute);
            OpenImageCommand = new DelegateCommand(OpenImage);
            ClearData();
            this.paymentRepository = paymentRepository;
        }

        private async void GetProviders()
        {
            ErrorText = string.Empty;
            IsButtonEnabled = false;

            try
            {
                ProvidersList.Clear();
                var list = await paymentRepository.GetNewMethods();
                foreach (var provider in list) ProvidersList.Add(provider);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private void OpenImage()
        {
            var image = FileDialogHelper.OpenFile(false);
            if (image.Any()) IconImage = image.First();
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!PaymentMethod.Validator() || IconImage.GetType() != typeof(BitmapImage))
            {
                ErrorText = "Please fill all data";
                return;
            }
            IsButtonEnabled = false;

            try
            {
                BitmapImage icon = (BitmapImage)IconImage;
                PaymentMethod.Photo = icon.GetBase64();

                (bool success, ErrorText) = await paymentRepository.AddPaymentMethod(PaymentMethod);
                IsButtonEnabled = true;
                if (success)
                {
                    ProvidersList.Remove(PaymentMethod.Provider);
                    ClearData();
                }
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private void ClearData()
        {
            IconImage = "pack://application:,,,/Images/AddIcon.png";
            PaymentMethod = new();
        }
    }
}
