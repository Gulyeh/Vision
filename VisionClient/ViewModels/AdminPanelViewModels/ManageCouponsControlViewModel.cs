using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class ManageCouponsControlViewModel : BindableBase
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


        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly ICouponRepository couponRepository;

        public DelegateCommand<DetailedCouponModel> EditCouponCommand { get; }
        public DelegateCommand<string> DeleteCouponCommand { get; }
        public DelegateCommand ExecuteCommand { get; }
        public ObservableCollection<DetailedCouponModel> CodesList { get; }

        public ManageCouponsControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,
            IDialogService dialogService, ICouponRepository couponRepository)
        {
            EditCouponCommand = new DelegateCommand<DetailedCouponModel>(EditCoupon);
            DeleteCouponCommand = new DelegateCommand<string>(DeleteCoupon);
            ExecuteCommand = new DelegateCommand(GetCoupons);

            CodesList = new();

            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.couponRepository = couponRepository;
            eventAggregator.GetEvent<SendEvent<string>>().Subscribe(x =>
            {
                GetCoupons();
            }, ThreadOption.PublisherThread, false, x => x.Equals("UpdateCoupons"));
        }


        private async void GetCoupons()
        {
            ErrorText = string.Empty;
            LoadingVisibility = Visibility.Visible;
            try
            {
                CodesList.Clear();
                var list = await couponRepository.GetCoupons();
                if(list.Any() && list is not null) CodesList.AddRange(list);
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async void DeleteCoupon(string code)
        {
            bool result = false;
            dialogService.ShowDialog("ConfirmControl", new DialogParameters
                {
                    { "title", "Confirm Delete" },
                    { "message", "Are you sure, you want to delete this coupon?" }
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
                (bool success, ErrorText) = await couponRepository.DeleteCoupon(code);
                if (success) CodesList.Remove(CodesList.First(x => x.Code == code));    
               
                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private void EditCoupon(DetailedCouponModel model)
        {
            regionManager.RequestNavigate("AdminPanelRegion", "EditCouponControl");
            eventAggregator.GetEvent<SendEvent<DetailedCouponModel>>().Publish(model);
        }
    }
}
