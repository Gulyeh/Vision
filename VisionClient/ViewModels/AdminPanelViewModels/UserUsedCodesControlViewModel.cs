using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;
using VisionClient.Helpers;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class UserUsedCodesControlViewModel : BindableBase
    {
        private string errorText = string.Empty;
        public string ErrorText
        {
            get { return errorText; }
            set { SetProperty(ref errorText, value); }
        }

        private Visibility loadingVisibility = Visibility.Collapsed;
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set { SetProperty(ref loadingVisibility, value); }
        }


        private readonly IDialogService dialogService;
        private readonly ICouponRepository couponRepository;

        private Guid UserId { get; set; }
        public ObservableCollection<UsedCodeModel> CodesList { get; set; } = new();

        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand<Guid?> DeleteCouponCommand { get; }

        public UserUsedCodesControlViewModel(IEventAggregator eventAggregator, IDialogService dialogService, ITextEventHelper panelOpened, ICouponRepository couponRepository)
        {
            ExecuteCommand = new DelegateCommand(GetUserUsedCodes);
            DeleteCouponCommand = new DelegateCommand<Guid?>(DeleteUsedCoupon);

            eventAggregator.GetEvent<SendEvent<(DetailedUserModel, string)>>().Subscribe(x =>
            {
                UserId = x.Item1.UserId;
            }, ThreadOption.PublisherThread, false, x => x.Item2.Equals("UserUsedCodes"));

            eventAggregator.GetEvent<SendEvent<DetailedUserModel>>().Subscribe(x =>
            {
                ErrorText = string.Empty;
                UserId = x.UserId;
            });

            panelOpened.Notify("UserUsedCodes");
            this.dialogService = dialogService;
            this.couponRepository = couponRepository;
        }

        private async void GetUserUsedCodes()
        {
            ErrorText = string.Empty;
            if (UserId == Guid.Empty) return;
            LoadingVisibility = Visibility.Visible;
            try
            {
                CodesList.Clear();
                var list = await couponRepository.GetUserUsedCoupons(UserId);
                CodesList.AddRange(list);
                ErrorText = $"Found {list.Count} records";

                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async void DeleteUsedCoupon(Guid? codeId)
        {
            ErrorText = string.Empty;
            if (codeId is null || codeId == Guid.Empty) return;

            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
            {
                { "title", "Confirm Delete" },
                { "message", "Are you sure, you want delete this?" }
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

            LoadingVisibility = Visibility.Visible;

            try
            {
                (bool success, ErrorText) = await couponRepository.DeleteUsedCoupon((Guid)codeId);
                if (success) CodesList.Remove(CodesList.First(x => x.Id == codeId));
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }
    }
}
