using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using VisionClient.Core.Dtos;
using VisionClient.Core.Events;
using VisionClient.Core.Models;
using VisionClient.Core.Repository.IRepository;

namespace VisionClient.ViewModels.AdminPanelViewModels
{
    internal class EditCouponControlViewModel : BindableBase
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

        private EditCouponDto couponModel = new();
        public EditCouponDto CouponModel
        {
            get { return couponModel; }
            set { SetProperty(ref couponModel, value); }
        }

        public DelegateCommand ExecuteCommand { get; }
        public DelegateCommand BackwardCommand { get; }

        private readonly IEventAggregator eventAggregator;
        private readonly IRegionManager regionManager;
        private readonly ICouponRepository couponRepository;

        public EditCouponControlViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, ICouponRepository couponRepository)
        {
            ExecuteCommand = new DelegateCommand(Execute);
            BackwardCommand = new DelegateCommand(NavigateToCoupons);
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.couponRepository = couponRepository;

            eventAggregator.GetEvent<SendEvent<DetailedCouponModel>>().Subscribe(x =>
            {
                CouponModel = new EditCouponDto()
                {
                    Id = x.Id,
                    Code = x.Code,
                    ExpireDate = x.ExpireDate,
                    IsLimited = x.IsLimited,
                    Uses = x.Uses
                };
            });
        }

        private async void Execute()
        {
            ErrorText = string.Empty;
            if (!CouponModel.Validator())
            {
                ErrorText = "Please fill all needed data";
                return;
            }
            IsButtonEnabled = false;

            try
            {
                (bool success, ErrorText) = await couponRepository.UpdateCoupon(CouponModel);
                if (success)
                {
                    eventAggregator.GetEvent<SendEvent<string>>().Publish("UpdateCoupons");
                    NavigateToCoupons();
                }
                IsButtonEnabled = true;
            }
            catch (Exception)
            {
                ErrorText = "Something went wrong";
                IsButtonEnabled = true;
            }
        }

        private void NavigateToCoupons() => regionManager.RequestNavigate("AdminPanelRegion", "ManageCouponsControl");
    }
}
