using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using VisionClient.Core.Dtos;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Extensions;
using VisionClient.Utility;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManagePaymentControlViewModel : BindableBase
    {
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

        private PaymentMethod paymentMethod = new();
        public PaymentMethod PaymentMethod
        {
            get { return paymentMethod; }
            set
            {
                SetProperty(ref paymentMethod, value);
                if (value is not null) IconImage = value.PhotoUrl;
            }
        }


        private readonly IPaymentRepository paymentRepository;
        private readonly IDialogService dialogService;

        public ObservableCollection<PaymentMethod> ProvidersList { get; set; } = new();
        public DelegateCommand ExecuteUpdateCommand { get; }
        public DelegateCommand ExecuteDeleteCommand { get; }
        public DelegateCommand OpenImageCommand { get; }
        public DelegateCommand ExecuteCommand { get; }

        public ManagePaymentControlViewModel(IPaymentRepository paymentRepository, IDialogService dialogService)
        {
            ExecuteUpdateCommand = new DelegateCommand(GetProviders);
            ExecuteCommand = new DelegateCommand(Execute);
            OpenImageCommand = new DelegateCommand(OpenImage);
            ExecuteDeleteCommand = new DelegateCommand(DeleteProvider);
            this.paymentRepository = paymentRepository;
            this.dialogService = dialogService;
        }

        private void OpenImage()
        {
            var image = FileDialogHelper.OpenFile(false);
            if (image.Any()) IconImage = image.First();
        }

        private async void GetProviders()
        {
            ErrorText = string.Empty;
            IsButtonEnabled = false;

            try
            {
                ProvidersList.Clear();
                var list = await paymentRepository.GetPaymentMethods();
                foreach (var provider in list) ProvidersList.Add(provider);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private async void DeleteProvider()
        {
            ErrorText = string.Empty;
            if (PaymentMethod.Id == Guid.Empty) return;
            bool result = false;

            dialogService.ShowDialog("ConfirmControl", new DialogParameters
                {
                    { "title", "Confirm Delete" },
                    { "message", "Are you sure, you want to delete this provider?" }
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
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await paymentRepository.DeleteMethod(PaymentMethod.Id);
                if (success)
                {
                    ProvidersList.Remove(PaymentMethod);
                    ClearData();
                }
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (PaymentMethod.Id == Guid.Empty) return;
            IsButtonEnabled = false;

            try
            {
                var editPayment = new EditPaymentDto()
                {
                    Id = PaymentMethod.Id,
                    IsAvailable = PaymentMethod.IsAvailable
                };

                if (IconImage.GetType() == typeof(BitmapImage))
                {
                    var bitmap = (BitmapImage)IconImage;
                    editPayment.Photo = bitmap.GetBase64();
                }

                ErrorText = await paymentRepository.UpdatePaymentMethod(editPayment);
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                IsButtonEnabled = true;
                ErrorText = "Something went wrong";
            }
        }

        private void ClearData()
        {
            PaymentMethod = new();
            IconImage = new();
        }
    }
}
